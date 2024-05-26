
public class Jump : ActionComponent
{
    public bool wallJump = false;
    public bool doubleJumpable = false;
    public float speedBonus = 0.0f;
    private JumpConfiguration _configuration;
    
    public Jump(JumpConfiguration configuration)
    {
        order = 1;
        _configuration = configuration;
        tick = false;
    }
    
    public override void Perform()
    {
        if (_movement.isGrounded) //jump
        {
            PerformJump(_configuration.jumpSpeed, _movement.GetHorizontalSpeed());
            doubleJumpable = true;
        }
        else if ((_configuration.enableDoubleJump 
                 && doubleJumpable 
                 && (_movement.Player.State == PlayerState.JUMP) 
                 && _movement.velocity.y < _configuration.doubleJumpSpeedThreshold) || wallJump) //double jump || wall jump
        {
            if (wallJump)
            {
                wallJump = false;
                PerformJump(_configuration.doubleJumpSpeed, _movement.GetHorizontalSpeed() + speedBonus);
                return;
            }
            
            PerformJump(_configuration.doubleJumpSpeed, _movement.GetHorizontalSpeed());
            doubleJumpable = false;
        }
    }
    
    public override void Cancel()
    {
        _movement._horizontalMovement.AbandonProvidedMaxSpeed();
        tick = false;
        _movement._verticalMovement.CheckGround = true;
        _movement._verticalMovement.UpdateState = true;
        base.Cancel();
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
    
    private void PerformJump(float jumpSpeed, float providedHSpeed)
    {
        base.Perform();
        _movement.ChangePlayerState(PlayerState.JUMP);
        _movement._verticalMovement.yVelocity = jumpSpeed;
        _movement.isGrounded = false;
        tick = true;
        _movement._horizontalMovement.ProvideMaxSpeed(providedHSpeed);
        _movement._verticalMovement.CheckGround = false;
        _movement._verticalMovement.UpdateState = false;
    }
}
