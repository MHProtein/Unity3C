using UnityEngine;

public class HorizontalMovement
{
    private Movement _movement;
    private float m_maxSpeed;
    private float m_providedMaxSpeed;
    private float m_turnSmoothVelocity;
    private float m_inputMagnitude;
    private bool m_useProvidedMaxSpeed;
    private HorizontalMovementConfiguration _configuration;
    
    public bool Sprintable { set; get; }
    
    public HorizontalMovement(Movement movement, HorizontalMovementConfiguration configuration)
    {
        _movement = movement;
        _configuration = configuration;
        Sprintable = true;
    }
    
    public void Update(Vector2 input, bool isSprint)
    {
        Move(input, isSprint);
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

        _movement.velocity = _movement.Transform.right * input.x + _movement.Transform.forward * input.y;
        _movement.velocity.Normalize();
        _movement.velocity *= m_maxSpeed;
    }
    
    void SetMaxSpeed(Vector2 input, bool isSprint)
    {
        if (isSprint && Sprintable)
        {
            if (_movement.isGrounded)
            {
                m_maxSpeed = _configuration.maxSprintSpeed;
                _movement.ChangePlayerState(PlayerState.SPRINT);
            }

            return;
        }

        m_inputMagnitude = input.sqrMagnitude;
        if (m_inputMagnitude > float.Epsilon)
        {
            if (m_useProvidedMaxSpeed)
            {
                m_maxSpeed = m_providedMaxSpeed;
            }
            else if (m_inputMagnitude > _configuration.runThreshold)
            {
                m_maxSpeed = _configuration.maxRunningVelocity;
                if (_movement.isGrounded)
                    _movement.ChangePlayerState(PlayerState.RUN);
            }
            else
            {
                m_maxSpeed = _configuration.maxWalkingVelocity;
                if (_movement.isGrounded)
                    _movement.ChangePlayerState(PlayerState.WALK);
            }
        }
        else
        {
            if (_movement.isGrounded && !m_useProvidedMaxSpeed)
                _movement.ChangePlayerState(PlayerState.IDLE);
            m_maxSpeed = 0.0f;
        }
    }
    
}
