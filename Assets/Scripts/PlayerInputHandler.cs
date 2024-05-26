using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    public Vector2 movementInput;
    public Vector2 cameraInput;
    public bool sprint;
    
    private Player _player;
    private PlayerInputActions _playerInputActions;

    public PlayerInputHandler(Player player)
    {
        _player = player;
        _playerInputActions = new PlayerInputActions();
    }
    
    public void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Jump.performed += OnJumpPerformed;
        _playerInputActions.Player.Crouch.performed += OnCrouchPerformed;
        _playerInputActions.Player.Glide.performed += OnGlidePerformed;
        _playerInputActions.Player.Glide.canceled += OnGlideCanceled;
        _playerInputActions.Player.Slide.performed += OnSlidePerformed;
        _playerInputActions.Player.Slide.canceled += OnSlideCanceled;
    }
    
    public void OnDisable()
    {
        _playerInputActions.Disable();
        _playerInputActions.Player.Jump.performed -= OnJumpPerformed;
        _playerInputActions.Player.Crouch.performed -= OnCrouchPerformed;
        _playerInputActions.Player.Glide.performed -= OnGlidePerformed;
        _playerInputActions.Player.Glide.canceled -= OnGlideCanceled;
        _playerInputActions.Player.Slide.performed -= OnSlidePerformed;
        _playerInputActions.Player.Slide.canceled -= OnSlideCanceled;
    }

    public void Update()
    {
        movementInput = _playerInputActions.Player.Movement.ReadValue<Vector2>();
        cameraInput = _playerInputActions.Player.CameraControl.ReadValue<Vector2>();
        sprint = _playerInputActions.Player.Sprint.ReadValue<float>() > 0.5f;
    }

    private void OnSlideCanceled(InputAction.CallbackContext context)
    {
        _player.slide.Cancel();
    }

    private void OnSlidePerformed(InputAction.CallbackContext context)
    {
        _player.slide.Perform();
    }

    private void OnGlideCanceled(InputAction.CallbackContext context)
    {
        _player.glide.Cancel();
    }

    private void OnGlidePerformed(InputAction.CallbackContext context)
    {
        _player.glide.Perform();
    }
    
    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        _player.crouch.Perform();
    }
    
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _player.jump.Perform();
    }
}
