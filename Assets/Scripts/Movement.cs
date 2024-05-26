using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Movement
{
    [HideInInspector] public Vector3 velocity = Vector3.zero; 
    [HideInInspector] public bool isGrounded = true; 
    public HorizontalMovement _horizontalMovement;
    public VerticalMovement _verticalMovement;
    
    private List<ActionComponent> components;
    
    private float m_maxSpeed;
    private float m_turnSmoothVelocity;
    
    public Transform Camera { private set; get; }
    public Transform Transform { private set; get; }
    public Player Player { private set; get; }
    public CharacterController Controller { private set; get; }
    
    public void AddComponent(ActionComponent component)
    {
        components.Add(component);
        component.SetMovement(this);
        components.Sort((component1, component2) => component1.order.CompareTo(component2.order));
    }
    
    public Movement(Player player, Transform transform, CharacterController controller, Transform camera,
        HorizontalMovementConfiguration hconfiguration,
        VerticalMovementConfiguration vconfiguration)
    {
        Player = player;
        Transform = transform;
        Controller = controller;
        Camera = camera;

        _horizontalMovement = new HorizontalMovement(this, hconfiguration);
        _verticalMovement = new VerticalMovement(this, vconfiguration);
        components = new List<ActionComponent>();
    }
    
    public void Update(Vector2 input, bool isSprint)
    {
        _horizontalMovement.Update(input, isSprint);
        _verticalMovement.Update();
        foreach (var component in components)
        {
            if(!component.tick)
                continue;
            component.FixedUpdate();
        }
        
        Controller.Move(velocity * Time.fixedDeltaTime);
    }

    public void ChangePlayerState(PlayerState newState)
    {
        Player.ChangePlayerState(newState);
    }

    public float GetHorizontalSpeed()
    {
        return new Vector2(velocity.x, velocity.z).magnitude;
    }
    
}
