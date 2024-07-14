using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity3C.StateMachine
{
    
    [CreateAssetMenu(fileName = "WallRun", menuName = "State/WallRun")]
    public class WallRun : State
    {
        [Range(0, 45)] public float cameraSlopeAngle;
        [Range(0, 5)] public float cameraOffset;     
        private bool isLeft;

        private int WALLRUN_LEFT = Animator.StringToHash("WallRunLeft");

        private int WALLRUN_RIGHT = Animator.StringToHash("WallRunRight");

        public override void Enter(params object[] enterParams)
        {
            base.Enter(enterParams);

            Dictionary<string, object> temp;
            temp = (Dictionary<string, object>)enterParams[0];
            
            _player.attributes.StaminaRechargable = false;
            isLeft = (bool)temp["IsLeft"];
            if(isLeft)
            {
                _player.animator.SetBool(WALLRUN_LEFT, true);
                _player.CameraControl.ChangeOffsetX(cameraOffset);
                _player.CameraControl.RotateCameraZ(-cameraSlopeAngle);
            }
            else
            {
                _player.animator.SetBool(WALLRUN_RIGHT, true);
                _player.CameraControl.ChangeOffsetX(-cameraOffset);
                _player.CameraControl.RotateCameraZ(cameraSlopeAngle);
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            _player.attributes.StaminaRechargable = true;
            if(isLeft)
                _player.animator.SetBool(WALLRUN_LEFT, false);
            else
                _player.animator.SetBool(WALLRUN_RIGHT, false);
            
            _player.CameraControl.RotateCameraZ(isLeft ? cameraSlopeAngle : -cameraSlopeAngle);
            _player.CameraControl.ChangeOffsetX(cameraOffset);
        }
    }
}