

public class Glide : ActionComponent
{
    private GlideConfiguration _configuration;
    private float m_liftValue;
    private float m_forwardSpeed;
    public Glide(GlideConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public override void Perform()
    {
        if (_movement.isGrounded || _movement.Player.State == PlayerState.WALLRUN)
            return;
        _movement.ChangePlayerState(PlayerState.GLIDE);
        tick = true;
        _movement._verticalMovement.isApplyGravity = false;
        base.Perform();
    }

    public override void Cancel()
    {
        _movement.ChangePlayerState(PlayerState.FALL);
        tick = false;
        _movement._verticalMovement.isApplyGravity = true;
        base.Cancel();
    }

    public override void FixedUpdate()
    {
        if (_movement.isGrounded)
        {
            _movement.ChangePlayerState(PlayerState.IDLE);
            tick = true;
            _movement._verticalMovement.isApplyGravity = true;
            return;
        }

        _movement.velocity.y = _configuration.modifiedYVelocity;
    }
}
