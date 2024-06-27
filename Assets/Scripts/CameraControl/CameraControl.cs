using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity3C.EventCenter;
using Unity3C.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity3C.CameraControl
{
    public enum ViewType
    {
        FirstPerson,
        ThirdPerson
    }
    
    [RequireComponent(typeof(Camera))]
    public class CameraControl : MonoBehaviour
    {
        
        public Transform _transform;
        public ViewType view = ViewType.ThirdPerson;
        [HideInInspector] public Camera camera;
        private float m_threshold = 0.01f;
        [SerializeField] private Player _player;
        
        
        [Space(10)] 
        [Header("First Person")]
        [Range(0, 90)]     public float topClamp = 90.0f;
        [Range(-90, 0)]     public float bottomClamp = -90.0f;
        [Range(0, 50)]     public float fpRotationSpeed = 1.0f;
        [Range(0, 50)]     public float cameraRotationSpeedZ = 10.0f;
        
        [SerializeField] private Camera _fpCamera;
        
        [Space(10)] 
        [Header("Third Person")]
        [SerializeField] private Camera _tpCamera;
     
        public Vector3 positionOffset = Vector3.up;
        [Range(-180, 180)] public float pitchMinAngle = -120.0f;
        [Range(-180, 180)] public float pitchMaxAngle = 90.0f;
        [Range(0, 1000)]   public float distance = 3.0f; 
        [Range(0, 10)]     public float freeMoveRadius = 1.0f;
        [Range(0, 1)]      public float focusCentering = 0.5f;
        [Range(0, 360)]    public float tpRotationSpeed = 90.0f;
        [Range(0, 20)]     public float alignDelay = 5.0f;
        [Range(0, 5)]     public float offsetChangeSmoothTime = 0.2f;
        [Range(0, 50)]     public float offsetChangeMaxSpeed = 5.0f;
        public LayerMask layerMask;
        
        private float m_rotationVelocity;
        private float m_targetPitch; 
        private float m_lastRotationTime;
        private Vector2 m_orbitAngles;
        private Vector3 m_focusPoint;
        private Vector3 m_previousFocusPoint;  
        private Vector3 m_loopDirection;
        private Vector3 m_lookPosition;  
        private Quaternion m_lookRotation;
        private RaycastHit hit;
        
        private float currentAngle;
        private float targetAngle;
        private bool isRotating;
        private float cameraRotationVelocity;
        
        private bool isOffsetChanging = false;
        private float targetOffset;
        private float offsetChangingVelocity;
        
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                halfExtends.y = camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                halfExtends.x = halfExtends.y * camera.aspect;
                halfExtends.z = 0.0f;
                
                return halfExtends;
            }
        }
        
        [SerializeField, Range(0f, 90f)]
        private float alignSmoothRange = 45f;
        private void Awake()
        {
            camera = _fpCamera;
            camera.enabled = true;
            _transform = camera.transform;
            view = ViewType.FirstPerson;
        }

        private void OnEnable()
        {
            PlayerInputHandler.Instance.playerInputActions.Player.CameraChange.performed += context => ChangeView();
        }

        public void ChangeView()
        {

            UnityEngine.Input.GetAxis("Horizontal");
            
            if (!_player.GetState().ViewChangable)
                return;
            camera.enabled = false;
            if (view != ViewType.FirstPerson)
            {
                camera = _fpCamera;
                camera.enabled = true;
                view = ViewType.FirstPerson;
            }
            else
            {
                camera = _tpCamera;
                camera.enabled = true;
                view = ViewType.ThirdPerson;
            }
            
            _transform = camera.transform;

            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "NewView", view },
            };
            
            RootEventCenter.Instance.Raise("OnViewChanged", messageDict);
        }
        
        public void ChangeOffsetX(float x)
        {
            isOffsetChanging = true;
            targetOffset = x;
        }

        private void ChangeOffsetX()
        {
            if (!isOffsetChanging)
                return;

            positionOffset.x = Mathf.SmoothDamp(positionOffset.x, targetOffset,
                ref offsetChangingVelocity, offsetChangeSmoothTime, offsetChangeMaxSpeed);
            if (Mathf.Abs(positionOffset.x - targetOffset) <= m_threshold)
            {
                isOffsetChanging = false;
            }
        }
    
        public void RotateCamera(Vector3 angle)
        {
            currentAngle = 0.0f;
            targetAngle = angle.z;
            isRotating = true;
        }

        private void RotateCamera()
        {
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, 
                cameraRotationSpeedZ * Time.fixedDeltaTime);
            if (Mathf.Abs(targetAngle - currentAngle) <= m_threshold)
            {
                isRotating = false;
            }
        }
        
        private void FixedUpdate()
        {
            CameraMovement(PlayerInputHandler.Instance.CameraInput);
        }
        
        private void CameraMovement(Vector2 input)
        {
            switch (view)
            {
                case ViewType.FirstPerson:
                    FirstPersonCameraMovement(input);
                    break;
                case ViewType.ThirdPerson:
                    ThirdPersonCameraMovement(input);
                    break;
            }
        }

        private void FirstPersonCameraMovement(Vector2 input)
        {
            if (input.sqrMagnitude >= m_threshold)
            {
                m_targetPitch += input.x * fpRotationSpeed * Time.fixedDeltaTime;
                m_rotationVelocity = input.y * fpRotationSpeed * Time.fixedDeltaTime;
                m_targetPitch = ClampAngle(m_targetPitch, bottomClamp, topClamp);
                
                _player.movementManager.Transform.Rotate(Vector3.up * m_rotationVelocity);
            }
            
            if (isRotating)
            {
                RotateCamera();
                _transform.localEulerAngles = new Vector3(m_targetPitch, camera.transform.localEulerAngles.y,
                    _transform.localEulerAngles.z + currentAngle);
            }
            else                
                _transform.localEulerAngles = new Vector3(m_targetPitch, camera.transform.localEulerAngles.y,
                    _transform.localEulerAngles.z);
        }

        private void ThirdPersonCameraMovement(Vector2 input)
        {
            ChangeOffsetX();
            UpdateFocusPoint(input);
            if (UpdateRotation(input) || AutoRotation())
            {
                m_orbitAngles.x = ClampAngle(m_orbitAngles.x, pitchMinAngle, pitchMaxAngle);
                m_lookRotation = Quaternion.Euler(m_orbitAngles);
            }
            else
            {
                m_lookRotation = _transform.localRotation;
            }
            m_loopDirection = m_lookRotation * Vector3.forward;
            m_lookPosition = m_focusPoint - m_loopDirection * distance + _transform.right * positionOffset.x;

            CollisionDetection();
            
            
            _transform.SetPositionAndRotation(m_lookPosition, m_lookRotation);
        }
        
        private void CollisionDetection()
        {
            if (Physics.BoxCast(m_focusPoint, CameraHalfExtends, -m_loopDirection, out hit, 
                    m_lookRotation, distance - camera.nearClipPlane, layerMask))
            {
                m_lookPosition = m_focusPoint - m_loopDirection * hit.distance;
            }
        }
        
        private void UpdateFocusPoint(Vector2 input)
        {
            m_previousFocusPoint = m_focusPoint;
            Vector3 targetPoint = _player.transform.position;
            targetPoint.y += positionOffset.y;
            if (freeMoveRadius > 0.0f)
            {
                float dist = Vector3.Distance(targetPoint, m_focusPoint);
                float t = 1.0f;
                if(dist > 0.01f && focusCentering > 0.0f)
                {
                    t = Mathf.Pow(1 - focusCentering, Time.fixedDeltaTime);
                    //t = 0.9862327f;
                }
                if (dist > freeMoveRadius)
                {
                    //m_focusPoint = Vector3.Lerp(targetPoint, m_focusPoint,
                    //  freeMoveRadius / distance);
                    t = Mathf.Min(t, freeMoveRadius / dist);
                }
                m_focusPoint = Vector3.Lerp(targetPoint, m_focusPoint, t);
            }
            else
            {
                m_focusPoint = targetPoint;
            }
        }

        private bool UpdateRotation(Vector2 input)
        {
            if (input.sqrMagnitude < float.Epsilon)
                return false;
            m_lastRotationTime = Time.fixedDeltaTime;
            m_orbitAngles += tpRotationSpeed * Time.fixedDeltaTime * input;
            return true;
        }

        private bool AutoRotation()
        {
            if (Time.fixedUnscaledTime - m_lastRotationTime < alignDelay)
                return false;

            Vector2 movement = new Vector2(m_focusPoint.x - m_previousFocusPoint.x,
                m_focusPoint.z - m_previousFocusPoint.z);
            
            if (movement.sqrMagnitude < 0.01f)
                return false;
            
            float headingAngle = GetAngle(movement.normalized);

            float rotationChangeRate = tpRotationSpeed * Time.fixedDeltaTime;
            float deltaAngleAbs = Mathf.Abs(Mathf.DeltaAngle(m_orbitAngles.y, headingAngle));

            if (deltaAngleAbs < alignSmoothRange)
            {
                rotationChangeRate *= deltaAngleAbs / alignSmoothRange;
            }
            else if (180.0f - deltaAngleAbs < alignSmoothRange)
            {
                rotationChangeRate *= (180.0f - deltaAngleAbs) / alignSmoothRange;
            }

            m_orbitAngles.y = Mathf.MoveTowardsAngle(m_orbitAngles.y, headingAngle, rotationChangeRate);
            return true;
        }

        private float GetAngle(Vector2 direction)
        {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return direction.x < 0.0f ? 360.0f - angle : angle;
        }
        
        private float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}