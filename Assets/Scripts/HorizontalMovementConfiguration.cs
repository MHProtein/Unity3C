using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Movement/HorizontalMovement", fileName = "HorizontalMovement")]
public class HorizontalMovementConfiguration : ScriptableObject
{
    public float maxWalkingVelocity = 3.0f;
    public float maxRunningVelocity = 6.0f;
    public float maxSprintSpeed = 10.0f;
    public float turnSmoothTime = 0.1f;
    public float runThreshold = 0.5f;
    private void Awake()
    {
        runThreshold *= runThreshold;
    }
}



