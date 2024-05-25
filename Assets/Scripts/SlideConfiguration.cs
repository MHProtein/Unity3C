
using UnityEngine;

[CreateAssetMenu(menuName = "Movement/Slide", fileName = "Slide")]
public class SlideConfiguration : ScriptableObject
{
    public float maxSlideTime = 2.0f;
    public float slideForce = 2.0f;
    public float maxSlideSpeed = 14.0f;
    public LayerMask layerMask;
}
