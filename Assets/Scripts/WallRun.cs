using System;
using UnityEngine;

[Serializable]
public class WallRun : ActionComponent
{
    public bool isWallRunning;
    public bool isLeft;
    
    private WallRunConfiguration _configuration;
    private Jump _jump;
    private RaycastHit m_leftHit;
    private RaycastHit m_rightHit;
    private RaycastHit m_runningHit;
    private bool m_snap;
    private float m_timer = 0.0f;
    private bool m_runable = true;
    private float m_originCameraSlopeAngle;
    private CameraControl _cameraControl;
    public WallRun(WallRunConfiguration configuration, Jump jump, CameraControl cameraControl)
    {
        order = 2;
        _configuration = configuration;
        _cameraControl = cameraControl;
        if (jump != null)
        {
            _jump = jump;
            _jump.speedBonus = configuration.jumpSpeedBonus;
            _jump.onActionPerformed += OnJumpPerformed;
            _jump.onActionCanceled += OnJumpCanceled;
        }
    }

    private void OnJumpCanceled()
    {
        if(!isWallRunning)
            tick = false;
    }
    
    private void OnJumpPerformed()
    {
        if (!isWallRunning)
        {
            tick = true; 
        }
        else
        {
            m_snap = false;
            ResetState();
        }
    }

    public override void Perform()
    { 
        isWallRunning = true;
        _jump.Cancel();
        _movement._horizontalMovement.ProvideMaxSpeed(Mathf.Clamp(_movement.GetHorizontalSpeed(),
            _configuration.minWallRunVelocity, _configuration.maxWallRunVelocity));
        _movement._verticalMovement.CheckGround = false;
        _movement._verticalMovement.ProvideGravity(_configuration.wallRunGravity);
        _movement._verticalMovement.yVelocity = 0.0f;
        _movement._verticalMovement.UpdateState = false;
        _jump.wallJump = true;
        _movement.ChangePlayerState(PlayerState.WALLRUN);
        m_snap = true;
        base.Perform();
    }

    public override void Cancel()
    {
        ResetState();
        tick = false;
        base.Cancel();
    }

    private void ResetState()
    {
        isWallRunning = false;
        _movement._horizontalMovement.AbandonProvidedMaxSpeed();
        _movement._verticalMovement.CheckGround = true;
        _movement._verticalMovement.AbandonProvidedGravity();
        _movement._verticalMovement.UpdateState = true;
        m_runable = false;
        m_snap = false;
        _cameraControl.RotateCamera(new Vector3(0.0f, 0.0f, 
            isLeft ? _configuration.cameraSlopeAngle : -_configuration.cameraSlopeAngle));
    }

    private void WallDetection()
    {
        if (Physics.Raycast(_movement.Transform.position + new Vector3(0.0f, _configuration.rayCastHeight, 0.0f),
                _movement.Transform.right,
                _configuration.rayCastDistance, _configuration.layerMask))
        {
            isLeft = false;
            Perform();
            _cameraControl.RotateCamera(new Vector3(0.0f, 0.0f, _configuration.cameraSlopeAngle));
        }
        else if (Physics.Raycast(_movement.Transform.position + new Vector3(0.0f, _configuration.rayCastHeight, 0.0f),
                     -_movement.Transform.right,
                     _configuration.rayCastDistance, _configuration.layerMask))
        {
            isLeft = true;
            Perform();
            _cameraControl.RotateCamera(new Vector3(0.0f, 0.0f, -_configuration.cameraSlopeAngle));
        }
    }
    
    private void CoolDown()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= _configuration.coolDown)
        {
            m_runable = true;
            m_timer = 0.0f;
        }
    }

    private void RunningRayCast(Vector3 direction)
    {
        if (Physics.Raycast(
                _movement.Transform.position + new Vector3(0.0f, _configuration.rayCastHeight, 0.0f),
                direction, out m_runningHit,
                _configuration.rayCastDistance, _configuration.layerMask))
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
            _movement.velocity = Vector3.ProjectOnPlane(_movement.velocity, m_runningHit.normal);
            _movement.velocity.y = tempy;
        }
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
}
