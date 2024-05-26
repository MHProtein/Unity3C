using UnityEngine;

public class Slide : ActionComponent
{
    private SlideConfiguration _configuration;
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

    public Slide(SlideConfiguration configuration, Jump jump, Crouch crouch)
    {
        order = 2;
        _configuration = configuration;
        
        if (jump != null)
        {
            _jump = jump;
            m_jumpAttached = true;
            _jump.onActionPerformed += JumpOnActionPerformed;
        }
        if (crouch != null)
        {
            _crouch = crouch;
            m_croughAttached = true;
        }
    }

    public override void Cancel()
    {
        if (!m_sliding)
            return;
        base.Cancel();
        if ( m_croughAttached && Physics.Raycast(_movement.Transform.position + Vector3.up, Vector3.up, 1.0f,
                _configuration.layerMask))
        {
            ResetState();
            _crouch.Perform();
            return;
        }
        else if (!m_croughAttached)
        {
            m_timer = 0.0f;
            return;
        }
        
        ResetState();
        _movement.ChangePlayerState(PlayerState.IDLE);
        if(!m_jumping)
            _movement._horizontalMovement.AbandonProvidedMaxSpeed();
    }

    public override void Perform()
    {
        if (_movement.Player.State == PlayerState.RUN || _movement.Player.State == PlayerState.SPRINT)
        {
            m_jumping = false;
            m_accumulatedVelocity = Vector3.zero;
            tick = true;
            m_sliding = true;
            m_timer = 0.0f;
            _movement.ChangePlayerState(PlayerState.SLIDE);
            _movement._horizontalMovement.ProvideMaxSpeed(_movement.GetHorizontalSpeed());
            _movement._horizontalMovement.Sprintable = false;
            _movement._verticalMovement.UpdateState = false;
            _movement._verticalMovement.CheckGround = false;
        }
        base.Perform();
    }

    public override void FixedUpdate()
    {
        if (Physics.Raycast(_movement.Transform.position, Vector3.down, out hit, 1.0f,
                _configuration.layerMask))
        {
            _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, hit.normal);
        }
        else
        {
            Cancel();
            return;
        }
        
        if (_movement.velocity.y > -0.1f && m_timer > _configuration.maxSlideTime)
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
        
        m_accumulatedVelocity += Time.fixedDeltaTime * _configuration.slideForce * _movement.velocity.normalized;
        _movement.velocity += m_accumulatedVelocity;
        
        if (hit.collider is not null)
        {
            _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, hit.normal);
        }
        
        ClampHorizontalSpeed();
    }
    
    private void JumpOnActionPerformed()
    {
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
        _movement.velocity = Vector3.ClampMagnitude(_movement.velocity, _configuration.maxSlideSpeed);
        _movement.velocity.y = m_tempY;
    }

    private void ResetState()
    {
        tick = false;
        m_sliding = false;
        _movement._verticalMovement.UpdateState = true;
        _movement._verticalMovement.CheckGround = true;
        _movement._horizontalMovement.Sprintable = false;
    }
}
