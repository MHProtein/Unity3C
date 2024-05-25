using UnityEngine;


[CreateAssetMenu(fileName = "Crouch", menuName = "Movement/Crouch", order = 0)]
public class CrouchConfiguration : ScriptableObject
{
    public float CrouchMoveSpeed = 2.0f;
    public Vector3 crouchColliderCenter = new Vector3(0.0f, 0.5f, 0.0f);
    public float crouchColliderRadius = 0.5f;
    public float crouchColliderHeight = 1.0f;
}