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
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            stateMachine = new PlayerStateMachine(this);
            attributes = new PlayerAttributes(attributesConfiguration);
            _inputHandler = new PlayerInputHandler();
            movementManager = new MovementManager(this);
            
            RootEventCenter.Instance.Register("SwitchState", OnSwitchState);
            
            stateText.text = "State : " + stateMachine.DefaultState;
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
            //apply stamina change

            _inputHandler.Update();
            attributes.Update();
            speedText.text = "HorizontalSpeed : " + movementManager.GetHorizontalSpeed().ToString("0.0")
                                                  + "\nVerticalSpeed : " + movementManager.velocity.y.ToString("0.0");
            staminaBar.fillAmount = attributes.Stamina / attributesConfiguration.maxStamina;
            
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

