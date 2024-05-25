using UnityEngine;

[CreateAssetMenu(menuName = "Movement/Attributes", fileName = "Attributes")]
public class AttributesConfiguration : ScriptableObject
{
    public float maxStamina = 100.0f;
    public float staminaRechargeRate = 5.0f;
}
