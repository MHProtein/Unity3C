using UnityEngine;

namespace Unity3C.StateMachine
{
    
    [CreateAssetMenu(fileName = "Sprint", menuName = "State/Sprint")]
    public class Sprint : State
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

        public override void Update()
        {
            base.Update();
        }
    }
}