using System;
using UnityEngine;

public class PlayerAttributes
{
    private AttributesConfiguration _configuration;
    public bool StaminaRechargable { set; get; }
    public float Stamina { get; private set; }

    public PlayerAttributes(AttributesConfiguration configuration)
    {
        _configuration = configuration;
        Stamina = _configuration.maxStamina;
    }

    public float ApplyStaminaChange(float delta)
    {
        Stamina += delta;
        if (Stamina < 0.0f)
            Stamina = 0.0f;
        else if (Stamina > _configuration.maxStamina)
            Stamina = _configuration.maxStamina;
        return Stamina;
    }

    private void RechargeStamina()
    {
        if (StaminaRechargable)
        {
            if (Stamina <= _configuration.maxStamina)
                Stamina += _configuration.staminaRechargeRate * Time.deltaTime;
        }
    }

    public void Update()
    {
        RechargeStamina();
    }
    
}
