using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Movement/WallRun", fileName = "WallRun")]
public class WallRunConfiguration : ScriptableObject
{
    public float rayCastDistance = 0.5f;
    public float rayCastHeight = 0.5f;
    public float wallRunGravity = 0.5f;
    public float maxWallRunVelocity = 10.0f;
    public float minWallRunVelocity = 8.0f;
    public float coolDown = 0.1f;
    public float jumpSpeedBonus = 3.0f;
    public LayerMask layerMask;
    public float cameraSlopeAngle = 7.0f;
}
