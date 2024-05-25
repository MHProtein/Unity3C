

using UnityEngine;
[CreateAssetMenu(menuName = "Movement/Jump", fileName = "Jump")]
public class JumpConfiguration : ScriptableObject
{
    public float jumpSpeed = 10.0f;
    public bool enableDoubleJump = true;
    public float doubleJumpSpeedThreshold = -0.2f;
    public float doubleJumpSpeed = 10.0f;
}
