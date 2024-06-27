
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3C.Movement
{
    public abstract class ActionComponent : ScriptableObject
    {
        public string actionName;
        public bool tick;
        protected MovementManager _movement;
        public int order = 0;
        //protected Dictionary<string, object> messageDict = new Dictionary<string, object>();
        protected virtual void Awake()
        {
            
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void Register()
        {
        }

        public virtual void Unregister()
        {
        }

        public virtual void Perform()
        {
        }
    
        public virtual void Start()
        {
        }
    
        public virtual void Cancel()
        {
        }

        public virtual void SetMovement(MovementManager movement)
        {
            _movement = movement;
        }
    
        public virtual void FixedUpdate()
        {
        }

        public Dictionary<string, object> CreateActionCancelDict()
        {
            return new Dictionary<string, object>()
            {
                { "ActionName", actionName },
                { "IsGrounded", _movement.isGrounded } 
            };
        }
    } 
}

