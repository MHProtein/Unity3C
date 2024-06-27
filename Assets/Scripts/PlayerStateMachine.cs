
using System.Collections.Generic;
using Unity3C.EventCenter;
using UnityEngine;

namespace Unity3C
{
    public class PlayerStateMachine : StateMachine.StateMachine
    {
        public PlayerStateMachine(Player player) : base(player)
        {
            
        }
        
        public void OnEnable()
        {
            RootEventCenter.Instance.Register("ActionPerform", OnActionPerform);
            RootEventCenter.Instance.Register("ActionCancel", OnActionCancel);
        }

        private void OnActionPerform(Dictionary<string, object> messageDict)
        {
            object actionName;
            if (!messageDict.TryGetValue("ActionName", out actionName))
                return;
            SwitchState((string)actionName, messageDict);
        }
        
        private void OnActionCancel(Dictionary<string, object> messageDict)
        {
            object isGrounded;
            if (!messageDict.TryGetValue("IsGrounded", out isGrounded))
                return;
            SwitchState(((bool)isGrounded ? "Idle" : "Fall"), messageDict);
        }

        public void OnDisable()
        {
            RootEventCenter.Instance.Unregister("ActionPerform", OnActionPerform);
            RootEventCenter.Instance.Unregister("ActionCancel", OnActionCancel);
        }
    }
}