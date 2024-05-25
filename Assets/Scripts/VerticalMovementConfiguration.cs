using UnityEngine;


[CreateAssetMenu(menuName = "Movement/VerticalMovement", fileName = "VerticalMovement")]
public class VerticalMovementConfiguration : ScriptableObject
{
    public float gravity = 20.0f;
    public LayerMask layerMask;
}