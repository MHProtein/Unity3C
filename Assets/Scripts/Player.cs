using System;
using System.Collections.Generic;
using TMPro;
using Unity3C.EventCenter;
using Unity3C.Input;
using Unity3C.Movement;
using Unity3C.StateMachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Unity3C
{

    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        public AttributesConfiguration attributesConfiguration;

        public float sprintStaminaCost = 0.5f;

        public PlayerStateMachine stateMachine;
        [HideInInspector] public MovementManager movementManager;
        
        [HideInInspector] public PlayerAttributes attributes;
        
        public CharacterController controller;
        public Animator animator;
        public PlayerInputHandler _inputHandler;
        public List<ActionComponent> ActionComponents;
        [FormerlySerializedAs("States")] public List<State> preRegisterStates;
        [FormerlySerializedAs("initState")] public State defaultState;
        
        [SerializeField] public TMP_Text stateText;
        [SerializeField] public TMP_Text speedText;
        [SerializeField] public Image staminaBar;
        [SerializeField] private Collider _collider;
        [SerializeField] public CameraControl.CameraControl CameraControl;

        private int _isStrafingHash = Animator.StringToHash("IsStrafing");
        private int _forwardStrafeHash = Animator.StringToHash("ForwardStrafe");
        private int _strafeDirectionXHash = Animator.StringToHash("StrafeDirectionX");
        private int _strafeDirectionZHash = Animator.StringToHash("StrafeDirectionZ");
        private int _moveSpeedHash = Animator.StringToHash("MoveSpeed");

        private const float ANIMATION_DAMP_TIME = 5.0f;
        private float m_strafeDirectionX = 0.0f;
        private float m_strafeDirectionZ = 0.0f;
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();

            stateMachine = new PlayerStateMachine(this);
            attributes = new PlayerAttributes(attributesConfiguration);
            _inputHandler = new PlayerInputHandler();
            movementManager = new MovementManager(this);
            
            RootEventCenter.Instance.Register("SwitchState", OnSwitchState);
            
            stateText.text = "State : " + stateMachine.DefaultState;
        }

        private void Start()
        {
            animator.SetFloat(_isStrafingHash, 1.0f);
        }

        private void OnSwitchState(Dictionary<string, object> messageDict)
        {
            StateMachine.State state = (State)messageDict["NewState"];
            stateText.text = "State : " + state.StateName;
        }

        public float ApplyStaminaChange(float delta)
        {
            return attributes.ApplyStaminaChange(delta);
        }
        
        public State GetState()
        {
            return stateMachine.CurrentState;
        }
        
        private void FixedUpdate()
        {
            movementManager.FixedUpdate();
        }

        private void Update()
        {
            _inputHandler.Update();
   
            float moveSpeed = movementManager.GetHorizontalSpeed();
            attributes.Update();
            speedText.text = "HorizontalSpeed : " + moveSpeed.ToString("0.0")
                                                  + "\nVerticalSpeed : " + movementManager.velocity.y.ToString("0.0");
            staminaBar.fillAmount = attributes.Stamina / attributesConfiguration.maxStamina;
            
            //update animator
            animator.SetFloat(_strafeDirectionXHash, _inputHandler.MovementInput.x);
            animator.SetFloat(_strafeDirectionZHash, _inputHandler.MovementInput.y);
            animator.SetFloat(_moveSpeedHash, moveSpeed);
            animator.SetFloat(_forwardStrafeHash, _inputHandler.MovementInput.y);
        }

        private void LateUpdate()
        {
        }
        
        private void OnEnable()
        {
            _inputHandler.OnEnable();
            stateMachine.OnEnable();
        }
        
        private void OnDisable()
        {
            _inputHandler.OnDisable();
            stateMachine.OnDisable();
        }
    }
}

