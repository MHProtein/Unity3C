
using UnityEngine;

[CreateAssetMenu(menuName = "Movement/CameraControl", fileName = "CameraControl")]
public class CameraControlConfiguration : ScriptableObject
{    
    public float TopClamp = 90.0f;
    public float BottomClamp = -90.0f;
    public float RotationSpeed = 1.0f;
}
