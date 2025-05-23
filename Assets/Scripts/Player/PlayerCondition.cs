using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    private Condition health { get {return uiCondition.health;} }
    private Condition stamina { get {return uiCondition.stamina;} }
    

    void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (health.curValue <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        
    }

    public void Eat(float amount)
    {
        health.Add(amount);
    }

    public void Drink(float amount)
    {
        stamina.Add(amount);
    }

    public void UseStamina(float amount)
    {
        stamina.Subtract(amount);
    }

    public bool StaminaIsEmpty()
    {
        return stamina.curValue <= 0;
    }

    public bool CanUseStamina()
    {
        return (stamina.curValue / stamina.maxValue > 0.2f);
    }
}
