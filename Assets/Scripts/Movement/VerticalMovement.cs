using System.Collections.Generic;
using Unity3C.EventCenter;
using UnityEngine;

namespace Unity3C.Movement
{
    [CreateAssetMenu(menuName = "Movement/VerticalMovement", fileName = "VerticalMovement")]
    public class VerticalMovement : ActionComponent
    {
        public float gravity = 20.0f;
        public LayerMask layerMask;
        
        public RaycastHit hit;
        [HideInInspector] public bool isApplyGravity = true;
        [HideInInspector] public float yVelocity;
         public bool isFalling;
        
        private float m_providedGrivity;
        private bool m_useProvidedGrivity;
        private bool m_checkGround;
        
        public bool UpdateState { get; set; }

        public bool CheckGround { get; set; }

        protected override void Awake()
        {
            base.Awake();
            order = 0;
        }

        public override void SetMovement(MovementManager movement)
        {
            base.SetMovement(movement);
            CheckGround = true;
            UpdateState = true;
        }

        public void ProvideGravity(float maxSpeed)
        {
            m_providedGrivity = maxSpeed;
            m_useProvidedGrivity = true;
            isFalling = false;
        }

        public void AbandonProvidedGravity()
        {
            m_useProvidedGrivity = false;
        }
        
        public override void FixedUpdate()
        {
            AboveCheck();
            ApplyGravity();
            _movement.velocity.y = yVelocity;
            GroundCheck();
        }

        public override void Register()
        {
            base.Register();
            yVelocity = 0.0f;
            m_useProvidedGrivity = false;
        }

        private void GroundCheck()
        {
            if (!CheckGround || yVelocity > 0.0f)
                return;
            if (Physics.SphereCast(_movement.Transform.position + Vector3.up * (_movement.Controller.radius + Physics.defaultContactOffset),
                    _movement.Controller.radius - Physics.defaultContactOffset, Vector3.down,
                    out hit, 0.2f, layerMask))
            {
                _movement.isGrounded = true;
                _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, hit.normal);
                isFalling = false;
            }
            else
            {
                _movement.isGrounded = false;
            }
        }

        private void AboveCheck()
        {
            if (Physics.Raycast(_movement.Transform.position + Vector3.up * 2.0f,
                    Vector3.up, _movement.Controller.skinWidth + .05f))
            {
                yVelocity = 0.0f;
            }
        }
        
        private void ApplyGravity()
        {
            if (!isApplyGravity)
            {
                yVelocity = 0.0f;
                return;
            }
            //to accumulate gravity
            if (!_movement.isGrounded)
            {
                yVelocity -= (m_useProvidedGrivity ? m_providedGrivity : gravity) * Time.fixedDeltaTime;
                if (!m_useProvidedGrivity && yVelocity < 0.0f)
                {
                    if (UpdateState)
                        ChangePlayerStateToFall();
                    isFalling = true;
                }
            }
            else
            {
                yVelocity = 0.0f;
            }
        }
        
        private void ChangePlayerStateToFall()
        {
            if (isFalling)
                return;
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "ActionName", "Fall" },
            };
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
        }
    } 
}


