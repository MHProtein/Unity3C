using System.Collections.Generic;
using Unity3C.EventCenter;
using Unity3C.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Unity3C.Movement
{
    [CreateAssetMenu(menuName = "Movement/Glide", fileName = "Glide")]
    public class Glide : ActionComponent
    {
        public float modifiedYVelocity = -1.0f;
        
        private float m_liftValue;
        private float m_forwardSpeed;
    
        public override void Perform()
        {
            if (_movement.isGrounded)
                return;
            //_movement.ChangePlayerState(PlayerState.GLIDE);
            tick = true;
            _movement._verticalMovement.isApplyGravity = false;
            base.Perform();
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "ActionName", actionName },
            };
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
        }

        public override void Cancel()
        {
            //_movement.ChangePlayerState(PlayerState.FALL);
            tick = false;
            _movement._verticalMovement.isApplyGravity = true;
            base.Cancel();
            
            RootEventCenter.Instance.Raise("ActionCancel", CreateActionCancelDict());
        }

        public override void FixedUpdate()
        {
            if (_movement.isGrounded)
            {
                //_movement.ChangePlayerState(PlayerState.IDLE);
                tick = true;
                _movement._verticalMovement.isApplyGravity = true;
                return;
            }

            _movement.velocity.y = modifiedYVelocity;
        }

        public override void Register()
        {
            base.Register();
            PlayerInputHandler.Instance.playerInputActions.Player.Glide.performed += OnGlidePerformed;
            PlayerInputHandler.Instance.playerInputActions.Player.Glide.canceled += OnGlideCanceled;
        }

        public override void Unregister()
        {
            base.Unregister();
            PlayerInputHandler.Instance.playerInputActions.Player.Glide.performed -= OnGlidePerformed;
            PlayerInputHandler.Instance.playerInputActions.Player.Glide.canceled -= OnGlideCanceled;
        }

        private void OnGlideCanceled(InputAction.CallbackContext context)
        {
            Cancel();
        }

        private void OnGlidePerformed(InputAction.CallbackContext context)
        {
            Perform();
        }
    }
}


