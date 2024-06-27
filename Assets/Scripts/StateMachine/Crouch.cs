
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity3C.StateMachine
{
    [CreateAssetMenu(fileName = "Crouch", menuName = "State/Crouch")]
    public class Crouch : State
    {
        
        private int CROUCH = Animator.StringToHash("Crouch");
        
        public override void Enter(params object[] enterParams)
        {
            base.Enter(enterParams);
            _player.animator.SetBool(CROUCH, true);
        }
        
        public override void Exit()
        {
            base.Exit();
            _player.animator.SetBool(CROUCH, false);
        }
    }
}