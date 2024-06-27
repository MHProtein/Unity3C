﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity3C.StateMachine
{
    
    [CreateAssetMenu(fileName = "WallRun", menuName = "State/WallRun")]
    public class WallRun : State
    {
        [Range(0, 45)] public float cameraSlopeAngle;
        
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
                _player.CameraControl.ChangeOffsetX(1.0f);
                _player.CameraControl.RotateCamera(new Vector3(0.0f, 0.0f, -cameraSlopeAngle));
            }
            else
            {
                _player.animator.SetBool(WALLRUN_RIGHT, true);
                _player.CameraControl.ChangeOffsetX(-1.0f);
                _player.CameraControl.RotateCamera(new Vector3(0.0f, 0.0f, cameraSlopeAngle));
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
            
            _player.CameraControl.RotateCamera(new Vector3(0.0f, 0.0f, 
                isLeft ? cameraSlopeAngle : -cameraSlopeAngle));
            _player.CameraControl.ChangeOffsetX(1.0f);
        }
    }
}