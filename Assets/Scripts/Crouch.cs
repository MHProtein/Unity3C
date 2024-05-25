using System;
using UnityEngine;

[Serializable]
public class Crouch : ActionComponent
{
    private CrouchConfiguration _configuration;
    private bool m_toggled = false;
    
    public Crouch(CrouchConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override void Perform()
    {
        if (!m_toggled) //EnterCrouch
        {
            if (!_movement.isGrounded)
                return;
            if (_movement.Player.State == PlayerState.IDLE || _movement.Player.State == PlayerState.WALK
                                                           || _movement.Player.State == PlayerState.SLIDE)
            {
                EnterCrouch();
            }
        }
        else //ExitCrouch
        {
            ExitCrouch();
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

    private void EnterCrouch()
    {
        tick = true;
        m_toggled = true;
        _movement._horizontalMovement.ProvideMaxSpeed(_configuration.CrouchMoveSpeed);
        _movement.ChangePlayerState(PlayerState.CROUCH);
    }

    private void ExitCrouch()
    {
        tick = false;
        m_toggled = false;
        _movement._horizontalMovement.AbandonProvidedMaxSpeed();    
        _movement.ChangePlayerState(PlayerState.IDLE);
    }
}
