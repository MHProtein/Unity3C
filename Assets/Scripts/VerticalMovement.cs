using UnityEngine;

public class VerticalMovement
{
    public RaycastHit hit;
    public bool isApplyGravity = true;
    public float yVelocity;
    private VerticalMovementConfiguration _configuration;
    private Movement _movement;
    private float m_providedGrivity;
    private bool m_useProvidedGrivity;
    private bool m_checkGround;
    public bool UpdateState { get; set; }

    public bool CheckGround { get; set; }

    public VerticalMovement(Movement movement, VerticalMovementConfiguration configuration)
    {
        _movement = movement;
        _configuration = configuration;
        CheckGround = true;
    }
    
    public void ProvideGravity(float maxSpeed)
    {
        m_providedGrivity = maxSpeed;
        m_useProvidedGrivity = true;
    }

    public void AbandonProvidedGravity()
    {
        m_useProvidedGrivity = false;
    }
    
    public void Update()
    {
        AboveCheck();
        ApplyGravity();
        _movement.velocity.y = yVelocity;
        GroundedCheck();
    }
    
    void GroundedCheck()
    {
        if (!CheckGround || yVelocity > 0.0f)
            return;
        if (Physics.SphereCast(_movement.Transform.position + Vector3.up * (_movement.Controller.radius + Physics.defaultContactOffset),
                _movement.Controller.radius - Physics.defaultContactOffset, Vector3.down,
                out hit, 0.2f, _configuration.layerMask))
        {
            _movement.isGrounded = true;
            _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, hit.normal);
        }
        else
        {
            _movement.isGrounded = false;
        }
    }

    void AboveCheck()
    {
        if (Physics.Raycast(_movement.Transform.position + Vector3.up * 2.0f,
                Vector3.up, _movement.Controller.skinWidth + .05f))
        {
            _movement._verticalMovement.yVelocity = 0.0f;
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
            if(UpdateState && _movement.Player.State != PlayerState.JUMP)
                _movement.ChangePlayerState(PlayerState.FALL);
            yVelocity -= (m_useProvidedGrivity ? m_providedGrivity : _configuration.gravity) * Time.fixedDeltaTime;
        }
        else
        {
            yVelocity = 0.0f;
        }
    }
}
