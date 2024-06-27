using System.Collections.Generic;
using Unity3C.EventCenter;
using Unity3C.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity3C.Movement
{
    
    [CreateAssetMenu(fileName = "Crouch", menuName = "Movement/Crouch", order = 0)]
    public class Crouch : ActionComponent
    {
        public float CrouchMoveSpeed = 2.0f;
        
        private bool m_toggled = false;

        public override void Perform()
        {
            if (!m_toggled) //EnterCrouch
            {
                if (!_movement.isGrounded)
                    return;
                if (!_movement._horizontalMovement.isRunning)
                {
                    EnterCrouch();
                }
            }
            else //ExitCrouch
            {
                Cancel();
            }
            base.Perform();
        }
    
        public override void FixedUpdate()
        {
            if (!_movement.isGrounded)
            {
                Cancel();
            }
        }

        public override void Cancel()
        {
            ExitCrouch();
            base.Cancel();
        }

        public override void Register()
        {
            base.Register();
            PlayerInputHandler.Instance.playerInputActions.Player.Crouch.performed += OnCrouchPerformed;
        }

        public override void Unregister()
        {
            PlayerInputHandler.Instance.playerInputActions.Player.Crouch.performed -= OnCrouchPerformed;
        }
        
        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            Perform();
        }

        private void EnterCrouch()
        {
            tick = true;
            m_toggled = true;
            _movement._horizontalMovement.ProvideMaxSpeed(CrouchMoveSpeed);
            Dictionary<string, object> messageDict = new Dictionary<string, object>
            {
                { "ActionName", actionName },
            };
 
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
        }
        

        private void ExitCrouch()
        {
            tick = false;
            m_toggled = false;
            _movement._horizontalMovement.AbandonProvidedMaxSpeed();    
            //_movement.ChangePlayerState(PlayerState.IDLE);
            
            RootEventCenter.Instance.Raise("ActionCancel", CreateActionCancelDict());
        }
    }
}

