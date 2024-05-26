using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    IDLE,
    WALK,
    RUN,
    SPRINT,
    JUMP,
    FALL,
    CROUCH,
    GLIDE,
    SLIDE,
    WALLRUN
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public HorizontalMovementConfiguration hConfiguration;
    public VerticalMovementConfiguration vConfiguration;
    public JumpConfiguration jumpConfiguration;
    public CrouchConfiguration crouchConfiguration;
    public GlideConfiguration glideConfiguration;
    public SlideConfiguration slideConfiguration;
    public WallRunConfiguration wallRunConfiguration;
    public AttributesConfiguration attributesConfiguration;
    public CameraControlConfiguration cameraControlConfiguration;

    public float sprintStaminaCost = 0.5f;
    
    [HideInInspector] public Movement movement;
    [HideInInspector] public Jump jump;
    [HideInInspector] public Crouch crouch;
    [HideInInspector] public Glide glide;
    [HideInInspector] public Slide slide;
    [HideInInspector] public WallRun wallRun;
    [HideInInspector] public CameraControl cameraControl;
    [HideInInspector] public PlayerAttributes attributes;
    
    private CharacterController _controller;
    private Animator _animator;
    private PlayerInputHandler _inputHandler;
    
    [SerializeField] private Transform camera;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private Image staminaBar;
    
    private int CROUCH = Animator.StringToHash("Crouch");
    private int SLIDE = Animator.StringToHash("Slide");
    private int WALLRUN_LEFT = Animator.StringToHash("WallRunLeft");
    private int WALLRUN_RIGHT = Animator.StringToHash("WallRunRight");
    
    public PlayerState State { get; private set; }
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        attributes = new PlayerAttributes(attributesConfiguration);
        _inputHandler = new PlayerInputHandler(this);
        movement = new Movement(this, transform, _controller, 
            camera, hConfiguration, vConfiguration);
        cameraControl = new CameraControl(cameraControlConfiguration, movement, camera);
        jump = new Jump(jumpConfiguration);
        movement.AddComponent(jump);
        crouch = new Crouch(crouchConfiguration);
        movement.AddComponent(crouch);
        glide = new Glide(glideConfiguration);
        movement.AddComponent(glide);
        slide = new Slide(slideConfiguration, jump, crouch);
        movement.AddComponent(slide);
        wallRun = new WallRun(wallRunConfiguration, jump, cameraControl);
        movement.AddComponent(wallRun);
        stateText.text = "State : " + State;
    }

    public float ApplyStaminaChange(float delta)
    {
        return attributes.ApplyStaminaChange(delta);
    }
    
    public void ChangePlayerState(PlayerState newState)
    {
        if (State == newState)
            return;
        
        ExitState();
        EnterState(newState);
        
        State = newState;
        stateText.text = "State : " + State;
    }

    private void ExitState()
    {
        switch (State)
        {
            case PlayerState.SPRINT:
                attributes.StaminaRechargable = true;
                break;
            case PlayerState.CROUCH:
                _animator.SetBool(CROUCH, false);
                break;
            case PlayerState.JUMP:
                attributes.StaminaRechargable = true;
                break;
            case PlayerState.GLIDE:
                attributes.StaminaRechargable = true;
                break;
            case PlayerState.SLIDE:
            {
                attributes.StaminaRechargable = true;
                _animator.SetBool(SLIDE, false);
                break;
            }
            case PlayerState.WALLRUN:
            {
                attributes.StaminaRechargable = true;
                if(wallRun.isLeft)
                    _animator.SetBool(WALLRUN_LEFT, false);
                else
                    _animator.SetBool(WALLRUN_RIGHT, false);
                break;
            }
        }
    }

    private void EnterState(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.SPRINT:
                attributes.StaminaRechargable = false;
                break;
            case PlayerState.CROUCH:
                _animator.SetBool(CROUCH, true);
                break;
            case PlayerState.JUMP:
                attributes.StaminaRechargable = false;
                break;
            case PlayerState.GLIDE:
                attributes.StaminaRechargable = false;
                break;
            case PlayerState.SLIDE:
            {
                attributes.StaminaRechargable = false;
                _animator.SetBool(SLIDE, true);
                break;
            }
            case PlayerState.WALLRUN:
            {
                attributes.StaminaRechargable = false;
                if(wallRun.isLeft)
                    _animator.SetBool(WALLRUN_LEFT, true);
                else
                    _animator.SetBool(WALLRUN_RIGHT, true);
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        movement.Update(_inputHandler.movementInput, _inputHandler.sprint);
    }

    private void Update()
    {
        //apply stamina change
        if (State == PlayerState.SPRINT)
        {
            if (ApplyStaminaChange(-sprintStaminaCost * Time.deltaTime) <= 0.0f)
                movement._horizontalMovement.Sprintable = false;
            Debug.Log(attributes.Stamina);
        }

        if (!movement._horizontalMovement.Sprintable)
        {
            if(attributes.Stamina >= attributesConfiguration.maxStamina)
                movement._horizontalMovement.Sprintable = true;
        }
        
        _inputHandler.Update();
        attributes.Update();
        speedText.text = "HorizontalSpeed : " + movement.GetHorizontalSpeed()
                                              + "\nVerticalSpeed : " + movement.velocity.y;
        staminaBar.fillAmount = attributes.Stamina / attributesConfiguration.maxStamina;
    }

    private void LateUpdate()
    {
        cameraControl.LateUpdate(_inputHandler.cameraInput);
    }
    
    private void OnEnable()
    {
        _inputHandler.OnEnable();
    }
    
    private void OnDisable()
    {
        _inputHandler.OnDisable();
    }
}
