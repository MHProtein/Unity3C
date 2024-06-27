using System;
using System.Collections.Generic;
using Unity3C.EventCenter;
using UnityEngine;

namespace Unity3C.StateMachine
{
    public class StateMachine
    {
        private Player _player;
        public bool IsInitialized { get; private set; }
        public State DefaultState => defaultState;
        [SerializeField] private State defaultState;
        [SerializeField] private List<State> preRegisterStates = new List<State>();
        
        private List<State> RegisteredStateList => _registeredStateList;
        private List<State> _registeredStateList = new List<State>();

        public State CurrentState => currentState;
        private State currentState;
        
        public State LastState => lastState;
        private State lastState;

        public StateMachine(Player player)
        {
            _player = player;
            IsInitialized = true;
            defaultState = player.defaultState;
            preRegisterStates = player.preRegisterStates;
            preRegisterStates.ForEach(state => RegisterState(state));
        }

        public bool RegisterState(State state)
        {
            if (state == null)
                return false;
            if (!IsInitialized)
                return false;
            if (RegisteredStateList.Exists(s => s.StateName == state.StateName))
                return false;
            
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "Player", _player },
                { "RegisteredState", state },
                { "IsRegister", true }
            };
            RootEventCenter.Instance.Raise("PreRegisterState", messageDict);

            if ((bool)messageDict["IsRegister"])
            {
                RegisteredStateList.Add(state);
                state.Register(this, _player);
                
                messageDict.Clear();
                messageDict.Add("Player", _player);
                messageDict.Add("RegisterState", state);
                RootEventCenter.Instance.Raise("EntityRegisterState", messageDict);
                return true;
            }
            return false;
        }

        public bool UnRegisterState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return false;
            if (!IsInitialized)
                return false;
            var state = RegisteredStateList.Find(s => s.StateName == stateName);
            if (state is null)
                return false;
            
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "Player", _player },
                { "UnRegisteredState", state },
                { "IsUnRegister", true }
            };
            RootEventCenter.Instance.Raise("PreUnRegisterState", messageDict);

            if ((bool)messageDict["IsUnRegister"])
            {
                messageDict.Clear();
                messageDict.Add("Player", _player);
                messageDict.Add("UnRegisteredState", state);
                RootEventCenter.Instance.Raise("UnRegisterState", messageDict);
                return true;
            }

            return false;
        }

        public bool SwitchState(string targetStateName, params object[] args)
        {
            if (string.IsNullOrEmpty(targetStateName))
                return false;
            var state = RegisteredStateList.Find(s => s.StateName == targetStateName);
            if (state is null)
                return false;
            Dictionary<string, object> messageDict = new Dictionary<string, object>()
            {
                { "Player", _player },
                { "OldState", currentState },
                { "NewState", state },
                { "IsSwitch", true }
            };

            RootEventCenter.Instance.Raise("PreSwitchState", messageDict);
            
            if ((bool)messageDict["IsSwitch"])
            {
                messageDict.Clear();
                messageDict.Add("Player", _player);
                messageDict.Add("OldState", currentState);
                messageDict.Add("NewState", state);
                messageDict.Add("IsSwitch", true);

                if (currentState is not null)
                    currentState.Exit();
                lastState = currentState;
                currentState = state;
                currentState.Enter(args);
                
                RootEventCenter.Instance.Raise("SwitchState", messageDict);
                return true;
            }
            
            return false;
        }
    }
}