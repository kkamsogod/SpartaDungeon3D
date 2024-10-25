using System;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

public interface IWarmable
{
    void HeatUp(int heatAmount);
}

public class PlayerCondition : MonoBehaviour, IDamagable, IWarmable
{
    public UICondition UICondition;

    private Condition health => UICondition.health;
    private Condition hunger => UICondition.hunger;
    private Condition thirst => UICondition.thirst;
    private Condition cold => UICondition.cold;
    private Condition stamina => UICondition.stamina;

    public float NoHungerHealthDecay;
    public event Action OnTakeDamage;
    public event Action OnWarmHeal;

    private void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        cold.Subtract(cold.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.curValue == 0.0f | cold.curValue == 0.0f)
        {
            health.Subtract(NoHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0.0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Drink(float amount)
    {
        thirst.Add(amount);
    }

    public void Warm(float amount)
    {
        cold.Add(amount);
    }

    private void Die()
    {
        // 아직 구현 안함
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        OnTakeDamage?.Invoke();
    }

    public void HeatUp(int heatUPAmount)
    {
        cold.Add(heatUPAmount);
        OnWarmHeal?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Subtract(amount);
        return true;
    }
}