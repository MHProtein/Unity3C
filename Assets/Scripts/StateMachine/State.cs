using Unity3C;
using UnityEngine;

namespace Unity3C.StateMachine
{
    public abstract class State: ScriptableObject
    {
        public string StateName { get => stateName; set => stateName = value; }
        public bool ViewChangable = false;
        
        [SerializeField] protected string stateName;
        
        protected StateMachine stateMachine;
        protected Player _player;
        
        public virtual void Register(StateMachine stateMachine, Player player)
        {
            this.stateMachine = stateMachine;
            _player = player;
        }
        
        public virtual void Unregister() { }
        
        public virtual void Enter(params object[] enterParams) { }
        
        public virtual void Exit() { }
        
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
    }
}