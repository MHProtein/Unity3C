using System.Collections.Generic;
using Unity3C.EventCenter;
using Unity3C.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity3C.Movement
{
    [CreateAssetMenu(menuName = "Movement/Slide", fileName = "Slide")]
    public class Slide : ActionComponent
    {
        public float maxSlideTime = 2.0f;
        public float slideForce = 2.0f;
        public float maxSlideSpeed = 14.0f;
        public LayerMask layerMask;
        
        private Jump _jump;
        private Crouch _crouch;
        private RaycastHit hit;
        
        private float m_timer;
        private bool m_sliding;
        private Vector3 m_accumulatedVelocity;
        private bool m_jumping;
        private bool m_jumpAttached;
        private bool m_croughAttached;
        private float m_tempY;

        protected override void Awake()
        {
            base.Awake();
            order = 2;
        }

        public override void SetMovement(MovementManager movement)
        {
            base.SetMovement(movement);
            if (movement.GetComponent(out _jump))
            {
                m_jumpAttached = true;
                RootEventCenter.Instance.Register("ActionPerform", OnJumpPerformed);
            }
            if (movement.GetComponent(out _crouch))
            {
                m_croughAttached = true;
            }
        }

        public override void Perform()
        {
            if (_movement._horizontalMovement.isRunning)
            {
                m_jumping = false;
                m_accumulatedVelocity = Vector3.zero;
                tick = true;
                m_sliding = true;
                m_timer = 0.0f;
                //_movement.ChangePlayerState(PlayerState.SLIDE);
                _movement._horizontalMovement.ProvideMaxSpeed(_movement.GetHorizontalSpeed());
                _movement._horizontalMovement.Sprintable = false;
                _movement._verticalMovement.UpdateState = false;
                _movement._verticalMovement.CheckGround = false;
            }
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "ActionName", actionName },
            };
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
            base.Perform();
        }
        
        public override void Cancel()
        {
            if (!m_sliding)
                return;
            base.Cancel();
            if ( m_croughAttached && Physics.Raycast(_movement.Transform.position + Vector3.up, 
                    Vector3.up, 1.0f, layerMask))
            {
                Debug.Log("crouch");
                ResetState();
                _crouch.Perform();
                return;
            }
            if (!m_croughAttached)
            {
                m_timer = 0.0f;
                return;
            }
            
            ResetState();
            //_movement.ChangePlayerState(PlayerState.IDLE);
            if(!m_jumping)
                _movement._horizontalMovement.AbandonProvidedMaxSpeed();

            RootEventCenter.Instance.Raise("ActionCancel", CreateActionCancelDict());
            base.Cancel();
        }

        public override void FixedUpdate()
        {
            if (Physics.Raycast(_movement.Transform.position, Vector3.down, out hit, 1.0f,
                    layerMask))
            {
                _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, hit.normal);
            }
            else
            {
                Cancel();
                return;
            }
            
            if (_movement.velocity.y > -0.1f && m_timer > maxSlideTime)
            {
                Cancel();
                return;
            }
            if (_movement.velocity.y > 0.5f)
            {
                Cancel();
                return;
            }
            
            m_timer += Time.deltaTime;
            
            m_accumulatedVelocity += Time.fixedDeltaTime * slideForce * _movement.velocity.normalized;
            _movement.velocity += m_accumulatedVelocity;
            
            if (hit.collider is not null)
            {
                _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, hit.normal);
            }
            
            ClampHorizontalSpeed();
        }
        
        public override void Register()
        {
            base.Register();
            PlayerInputHandler.Instance.playerInputActions.Player.Slide.performed += OnSlidePerformed;
            PlayerInputHandler.Instance.playerInputActions.Player.Slide.canceled += OnSlideCanceled;
        }

        public override void Unregister()
        {
            base.Unregister();
            PlayerInputHandler.Instance.playerInputActions.Player.Slide.performed -= OnSlidePerformed;
            PlayerInputHandler.Instance.playerInputActions.Player.Slide.canceled -= OnSlideCanceled;
        }

        private void OnSlideCanceled(InputAction.CallbackContext context)
        {
            Cancel();
        }

        private void OnSlidePerformed(InputAction.CallbackContext context)
        {
            Perform();
        }
        
        private void OnJumpPerformed(Dictionary<string, object> messageDict)
        {
            if ((string)messageDict["ActionName"] != "Jump")
                return;
            if (m_sliding)
            {
                _movement._horizontalMovement.ProvideMaxSpeed(_movement.GetHorizontalSpeed());
                m_jumping = true;
                Cancel();
            }
        }
        
        private void ClampHorizontalSpeed()
        {
            m_tempY = _movement.velocity.y;
            _movement.velocity.y = 0.0f;
            _movement.velocity = Vector3.ClampMagnitude(_movement.velocity, maxSlideSpeed);
            _movement.velocity.y = m_tempY;
        }

        private void ResetState()
        {
            tick = false;
            m_sliding = false;
            _movement._verticalMovement.UpdateState = true;
            _movement._verticalMovement.CheckGround = true;
            _movement._horizontalMovement.Sprintable = true;
        }
    }
}

    