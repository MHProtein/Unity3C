
using System.Collections.Generic;
using Unity3C.EventCenter;
using Unity3C.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Unity3C.Movement
{
    public enum JumpType
    {
        Jump,
        DoubleJump,
        WallJump
    }
    
    [CreateAssetMenu(menuName = "Movement/Jump", fileName = "Jump")]
    public class Jump : ActionComponent
    {
        public float jumpSpeed = 10.0f;
        public bool enableDoubleJump = true;
        public float doubleJumpSpeedThreshold = -0.2f;
        public float doubleJumpSpeed = 10.0f;
        
        [HideInInspector] public bool wallJump = false;
        [HideInInspector] public bool doubleJumpable = false;
        [HideInInspector] public float speedBonus = 0.0f;
        [HideInInspector] public bool isJumping = false;
        protected override void Awake()
        {
            base.Awake();
            order = 1;
            tick = false;
        }

        public override void Perform()
        {
            JumpType jumpType = JumpType.Jump;
            float speed = 0.0f;
            if (_movement.isGrounded) //jump
            {
                speed = jumpSpeed;
                PerformJump(jumpSpeed, _movement.GetHorizontalSpeed());
                doubleJumpable = true;
                jumpType = JumpType.Jump;
            }
            else if (wallJump) //wall jump
            {
                wallJump = false;
                PerformJump(doubleJumpSpeed,  Mathf.Clamp(_movement.GetHorizontalSpeed(), 9.0f, 12.0f));
                jumpType = JumpType.WallJump;
            }
            else if ((enableDoubleJump 
                      && doubleJumpable
                      && !_movement.isGrounded 
                      && _movement.velocity.y < doubleJumpSpeedThreshold)) //double jump
            {
                speed = _movement.GetHorizontalSpeed();
                PerformJump(doubleJumpSpeed, speed);
                doubleJumpable = false;
                jumpType = JumpType.DoubleJump;
            }
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "ActionName", actionName},
                { "HorizontalSpeed", speed },
                { "JumpType", jumpType}
            };
            
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
        }
        
        public override void Cancel()
        {
            _movement._horizontalMovement.AbandonProvidedMaxSpeed();
            tick = false;
            _movement._verticalMovement.CheckGround = true;
            _movement._verticalMovement.UpdateState = true;
            isJumping = false;
            wallJump = false;
            base.Cancel();
            RootEventCenter.Instance.Raise("ActionCancel", CreateActionCancelDict());
        }

        public override void FixedUpdate()
        {
            if (_movement.isGrounded)
            {
                Cancel();
            }

            if (_movement.velocity.y < -0.1f) //check is falling
            {
                _movement._verticalMovement.CheckGround = true;
            }
        }

        public override void Register()
        {
            base.Register();
            PlayerInputHandler.Instance.playerInputActions.Player.Jump.performed += OnJumpPerformed;
        }
        
        public override void Unregister()
        {
            base.Unregister();
            PlayerInputHandler.Instance.playerInputActions.Player.Jump.performed -= OnJumpPerformed;
        }
        
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            Perform();
        }

        private void PerformJump(float jumpSpeed, float providedHSpeed)
        {
            base.Perform();
            _movement._verticalMovement.yVelocity = jumpSpeed;
            _movement.isGrounded = false;
            tick = true;
            
            _movement._horizontalMovement.ProvideMaxSpeed(Mathf.Clamp(providedHSpeed, 3.0f, providedHSpeed));
            _movement._verticalMovement.CheckGround = false;
            _movement._verticalMovement.UpdateState = false;
            isJumping = true;
        }
    }
}

