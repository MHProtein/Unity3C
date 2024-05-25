using UnityEngine;

public class CameraControl
{
    private CameraControlConfiguration _configuration;
    private Movement _movement;
    private Transform _camera;
    
    private float m_threshold = 0.01f;
    private float m_rotationVelocity;
    private float m_cinemachineTargetPitch;

    public CameraControl(CameraControlConfiguration configuration, Movement movement, Transform camera)
    {
        _configuration = configuration;
        _movement = movement;
        _camera = camera;
    }

    public void RotateCamera(Vector3 angle)
    {
        _camera.Rotate(angle);
    }
    
    public void LateUpdate(Vector2 input)
    {
        CameraRotation(input);
    }
    
    private void CameraRotation(Vector2 input)
    {
        if (input.sqrMagnitude >= m_threshold)
        {
				
            m_cinemachineTargetPitch += input.y * _configuration.RotationSpeed * Time.deltaTime;
            m_rotationVelocity = input.x * _configuration.RotationSpeed * Time.deltaTime;
            m_cinemachineTargetPitch = ClampAngle(m_cinemachineTargetPitch, 
                _configuration.BottomClamp, _configuration.TopClamp);

            _camera.localEulerAngles = new Vector3(m_cinemachineTargetPitch, _camera.localEulerAngles.y,
                _camera.localEulerAngles.z);
            _movement.Transform.Rotate(Vector3.up * m_rotationVelocity);
        }
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
