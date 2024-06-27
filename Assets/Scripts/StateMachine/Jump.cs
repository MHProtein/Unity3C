using UnityEngine;

namespace Unity3C.StateMachine
{
    
    [CreateAssetMenu(fileName = "Jump", menuName = "State/Jump")]
    public class Jump : State
    {
        public override void Enter(params object[] enterParams)
        {
            base.Enter(enterParams);
            _player.attributes.StaminaRechargable = false;
        }

        public override void Exit()
        {
            base.Exit();
            _player.attributes.StaminaRechargable = true;
        }
    }
}