using Unity3C;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity3C.Input
{
    public class PlayerInputHandler
    {
        public static PlayerInputHandler Instance;
        public Vector2 MovementInput => playerInputActions.Player.Movement.ReadValue<Vector2>();
        public Vector2 CameraInput
        {
            get
            {
                Vector2 temp = playerInputActions.Player.CameraControl.ReadValue<Vector2>();
                return new Vector2(temp.y, temp.x);
            }
        }

        public bool IsSprint => playerInputActions.Player.Sprint.IsPressed();
        
        [HideInInspector] public PlayerInputActions playerInputActions;
        
        public PlayerInputHandler()
        {
            playerInputActions = new PlayerInputActions();
            Instance = this;
        }
        
        public void OnEnable()
        {
            playerInputActions.Enable();
        }
        
        public void OnDisable()
        {
            playerInputActions.Disable();
        }

        public void Update()
        {

        }
    }
}


