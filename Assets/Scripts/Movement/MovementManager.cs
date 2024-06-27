using System;
using System.Collections.Generic;
using Unity3C;
using Unity3C.EventCenter;
using UnityEngine;

namespace Unity3C.Movement
{
    public class MovementManager
    {
        [HideInInspector] public Vector3 velocity = Vector3.zero; 
        [HideInInspector] public bool isGrounded = false; 
        public HorizontalMovement _horizontalMovement;
        public VerticalMovement _verticalMovement;
        
        private List<ActionComponent> components;
        
        private float m_maxSpeed;
        private float m_turnSmoothVelocity;
        
        public Transform Transform { private set; get; }
        public Player Player { private set; get; }
        public CharacterController Controller { private set; get; }
        
        public void RegisterComponents(ActionComponent[] components)
        {
            foreach (var component in components)
            {
                RegisterComponent(component);
            }
        }
        
        public void RegisterComponent(ActionComponent component)
        {
            components.Add(component);
            component.SetMovement(this);
            component.Register();
            components.Sort((component1, component2) => component1.order.CompareTo(component2.order));
            
            if (component is HorizontalMovement)
            {
                _horizontalMovement = (HorizontalMovement)component;
            }
            
            if (component is VerticalMovement)
            {
                _verticalMovement = (VerticalMovement)component;
            }
        }
        
        public void UnregisterComponents(ActionComponent[] components)
        {
            foreach (var component in components)
            {
                UnregisterComponent(component);
            }
        }
        
        public void UnregisterComponent(ActionComponent component)
        {
            components.Remove(component);
            component.Unregister();
            
            if (component is HorizontalMovement)
            {
                _horizontalMovement = null;
            }
            
            if (component is VerticalMovement)
            {
                _verticalMovement = null;
            }
        }
        
        public MovementManager(Player player)
        {
            Player = player;
            Transform = player.transform;
            Controller = player.controller;
            components = new List<ActionComponent>();
            RegisterComponents(player.ActionComponents.ToArray());
        }
        
        public bool GetComponent<T>(out T component) where T : ActionComponent
        {
            component = null;
            foreach (var Component in components)
            {
                if (Component is T)
                {
                    component = (T)Component;
                    return true;
                }
            }
            return false;
        }

        //todo: modify it to raise events
        public void SendActionPerformEvent(Dictionary<string, object> messageDict)
        {
            RootEventCenter.Instance.Raise("ActionPerform", messageDict);
        }

        public float GetHorizontalSpeed()
        {
            return new Vector2(velocity.x, velocity.z).magnitude;
        }
        
        public void FixedUpdate()
        {
            foreach (var component in components)
            {
                if(!component.tick)
                    continue;
                component.FixedUpdate();
            }
            
            Controller.Move(velocity * Time.fixedDeltaTime);
        }
    }
}

