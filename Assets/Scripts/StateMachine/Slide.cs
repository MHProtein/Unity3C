

using UnityEngine;

namespace Unity3C.StateMachine
{
    
    [CreateAssetMenu(fileName = "Slide", menuName = "State/Slide")]
    public class Slide : State
    {
        
        private int SLIDE = Animator.StringToHash("Slide");

        public override void Enter(params object[] enterParams)
        {
            base.Enter(enterParams);
            _player.attributes.StaminaRechargable = false;
            _player.animator.SetBool(SLIDE, true);
        }

        public override void Exit()
        {
            base.Exit();
            _player.attributes.StaminaRechargable = true;
            _player.animator.SetBool(SLIDE, false);
        }
    }
}