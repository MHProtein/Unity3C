using System.Collections.Generic;
using Unity3C.CameraControl;
using Unity3C.EventCenter;
using UnityEngine;

namespace Unity3C.Movement
{
    [CreateAssetMenu(menuName = "Movement/WallRun", fileName = "WallRun")]
    public class WallRun : ActionComponent
    {
        public float rayCastDistance = 0.5f;
        public float rayCastHeight = 0.5f;
        public float wallRunGravity = 0.5f;
        public float maxWallRunVelocity = 10.0f;
        public float minWallRunVelocity = 8.0f;
        public float coolDown = 0.1f;
        public float jumpSpeedBonus = 3.0f;
        public LayerMask layerMask;
        
        public bool isWallRunning;
        public bool isLeft;
        
        private Jump _jump;
        private RaycastHit _leftHit;
        private RaycastHit _rightHit;
        private RaycastHit _runningHit;
        
        private bool m_snap;
        private float m_timer = 0.0f;
        private bool m_runable = true;
        private float m_originCameraSlopeAngle;
        private bool m_jumpAssigned = false;

        protected override void Awake()
        {
            base.Awake();
            order = 2;
        }

        public override void SetMovement(MovementManager movement)
        {
            base.SetMovement(movement);
            
            if (movement.GetComponent<Jump>(out _jump))
            {
                m_jumpAssigned = true;
                _jump.speedBonus = jumpSpeedBonus;
            }
        }

        public override void Perform()
        { 
            isWallRunning = true;
            _jump.Cancel();
            _movement._horizontalMovement.ProvideMaxSpeed(Mathf.Clamp(_movement.GetHorizontalSpeed(),
                minWallRunVelocity, maxWallRunVelocity));
            _movement._verticalMovement.CheckGround = false;
            _movement._verticalMovement.ProvideGravity(wallRunGravity);
            _movement._verticalMovement.yVelocity = 0.0f;
            _movement._verticalMovement.UpdateState = false;
            _jump.wallJump = true;
            //_movement.ChangePlayerState(PlayerState.WALLRUN);
            m_snap = true;
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "ActionName", actionName },
                { "IsLeft", isLeft}
            };
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
            
            base.Perform();
        }

        public override void Register()
        {
            base.Register();
            RootEventCenter.Instance.Register("ActionPerform", OnJumpPerformed);
            RootEventCenter.Instance.Register("ActionCancel", OnJumpCanceled);
        }

        public override void Unregister()
        {
            base.Unregister();
            RootEventCenter.Instance.Unregister("ActionPerform", OnJumpPerformed);
            RootEventCenter.Instance.Unregister("ActionCancel", OnJumpCanceled);
        }

        public override void FixedUpdate()
        {
            if (!m_runable)
                CoolDown();
            else if (!isWallRunning)
                WallDetection();
            else
            {
                RunningRayCast(isLeft ? -_movement.Transform.right : _movement.Transform.right);
                SnapToWall();
            }
        }

        public override void Cancel()
        {
            ResetState();
            tick = false;

            RootEventCenter.Instance.Raise("ActionCancel", CreateActionCancelDict());
            base.Cancel();
        }
        
        private void OnJumpPerformed(Dictionary<string, object> messageDict)
        {
            if ((string)messageDict["ActionName"] != "Jump")
                return;
            if (!isWallRunning)
            {
                tick = true; 
            }
            else
            {
                Cancel();
                tick = true;
            }
        }

        private void ResetState()
        {
            isWallRunning = false;
            if (!_jump.isJumping)
            {
                _movement._horizontalMovement.AbandonProvidedMaxSpeed();
                _movement._verticalMovement.UpdateState = true;
            }
            _movement._verticalMovement.CheckGround = true;
            _movement._verticalMovement.AbandonProvidedGravity();
            m_runable = false;
            m_snap = false;
        }
        
        private void OnJumpCanceled(Dictionary<string, object> messageDict)
        {
            if ((string)messageDict["ActionName"] != "Jump")
                return;
            if(!isWallRunning)
                tick = false;
        }

        private void WallDetection()
        {
            if (Physics.Raycast(_movement.Transform.position + new Vector3(0.0f, rayCastHeight, 0.0f),
                    _movement.Transform.right,
                    rayCastDistance, layerMask))
            {
                isLeft = false;
                Perform();
            }
            else if (Physics.Raycast(_movement.Transform.position + new Vector3(0.0f, rayCastHeight, 0.0f),
                         -_movement.Transform.right,
                         rayCastDistance, layerMask))
            {
                isLeft = true;
                Perform();
            }
        }
        
        private void CoolDown()
        {
            m_timer += Time.deltaTime;
            if (m_timer >= coolDown)
            {
                m_runable = true;
                m_timer = 0.0f;
            }
        }

        private void RunningRayCast(Vector3 direction)
        {
            if (Physics.Raycast(
                    _movement.Transform.position + new Vector3(0.0f, rayCastHeight, 0.0f),
                    direction, out _runningHit,
                    rayCastDistance, layerMask))
            {
                return;
            }
            Cancel();
        }

        private void SnapToWall()
        {
            if (m_snap)
            {
                float tempy = _movement.velocity.y;
                _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, _runningHit.normal);
                _movement.velocity.y = tempy;
            }
        }
    }
}


