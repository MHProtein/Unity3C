using System;
using System.Collections.Generic;
using Unity3C;
using Unity3C.EventCenter;
using Unity3C.Input;
using UnityEngine;

namespace Unity3C.Movement
{
    [CreateAssetMenu(menuName = "Movement/HorizontalMovement", fileName = "HorizontalMovement")]
    public class HorizontalMovement : ActionComponent
    {
        public float maxWalkingVelocity = 3.0f;
        public float maxRunningVelocity = 6.0f;
        public float maxSprintSpeed = 10.0f;
        public float runThreshold = 0.5f;
        [HideInInspector] public bool isRunning;
        private float m_maxSpeed;
        private float m_providedMaxSpeed;
        private float m_turnSmoothVelocity;
        private float m_inputMagnitude;
        private bool m_useProvidedMaxSpeed;
        private bool sprintable = true;
        private string currentState = "Idle";
        
        public bool Sprintable
        {
            get => sprintable;
            set => sprintable = value;
        }

        private void Awake()
        {
            order = 0;
            sprintable = true;
            runThreshold *= runThreshold;
        }

        public override void Register()
        {
            base.Register();
            RootEventCenter.Instance.Register("OnViewChanged", OnViewChanged);
        }

        private void OnViewChanged(Dictionary<string, object> messageDict)
        {
            
        }

        public override void FixedUpdate()
        {
            Move(PlayerInputHandler.Instance.MovementInput, PlayerInputHandler.Instance.IsSprint);
        }

        public void ProvideMaxSpeed(float maxSpeed)
        {
            m_providedMaxSpeed = maxSpeed;
            m_useProvidedMaxSpeed = true;
        }

        public void AbandonProvidedMaxSpeed()
        {
            m_useProvidedMaxSpeed = false;
        }
        
        private void Move(Vector2 input, bool isSprint)
        {
            
            _movement.velocity = new Vector3(input.x, .0f, input.y).normalized;
            SetMaxSpeed(input, isSprint);
            if (m_maxSpeed < float.Epsilon)
                return;
            _movement.Player.transform.rotation = Quaternion.Euler(0.0f, _movement.Player.CameraControl._transform.eulerAngles.y, 0.0f);
            _movement.velocity = _movement.Transform.right * input.x + _movement.Transform.forward * input.y;
            _movement.velocity.Normalize();
            _movement.velocity *= m_maxSpeed;
        }
        
        private void SetMaxSpeed(Vector2 input, bool isSprint)
        {
            if (isSprint && sprintable)
            {
                if (_movement.isGrounded)
                {
                    m_maxSpeed = maxSprintSpeed;
                    isRunning = true;
                    ChangePlayerState("Sprint");
                    return;
                }
            }

            m_inputMagnitude = input.sqrMagnitude;
            if (m_inputMagnitude > float.Epsilon)
            {
                if (m_useProvidedMaxSpeed)
                {
                    isRunning = false;
                    m_maxSpeed = m_providedMaxSpeed;
                }
                else if (m_inputMagnitude > runThreshold)
                {
                    m_maxSpeed = maxRunningVelocity;
                    if (_movement.isGrounded)
                        isRunning = true;
                    ChangePlayerState("Run");
                }
                else
                {
                    m_maxSpeed = maxWalkingVelocity;
                    if (_movement.isGrounded)
                        isRunning = false;
                    ChangePlayerState("Walk");
                }
            }
            else
            {
                if (_movement.isGrounded && !m_useProvidedMaxSpeed)
                    isRunning = false;
                ChangePlayerState("Idle");
                m_maxSpeed = 0.0f;
            }
        }

        private void ChangePlayerState(string newState)
        {
            if (newState == currentState || !_movement.isGrounded)
                return;
            currentState = newState;
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "ActionName", currentState },
            };
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
        }
    }
}


