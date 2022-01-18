using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public float armour;
    public float maxArmour;
    public float absorbtion;
    public float taken { get { return 1 - absorbtion; } }
    public bool damageable;
    public bool dead;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Knockback(float damage, Vector3 direction, float knockback)
    {
        if(rb == null || direction == default(Vector3) || knockback == 0) return;

        rb.AddForce(direction*damage*knockback);
    }

    public virtual void OnDamage(float damage, Vector3 direction, float knockback)
    {

    }

    public virtual void OnKill(float damage, Vector3 direction)
    {
        Destroy(gameObject);
    }

    public virtual void OnHeal(float health)
    {

    }

    public virtual void OnGainArmour(float amount)
    {

    }

    public bool Damage(float damage, Vector3 direction=default(Vector3), float knockback=0)
    {
        float actual = damage*taken;
        if(damageable && !dead)
        {
            armour -= damage*absorbtion;
            if(armour < 0) armour = 0;
            health -= actual;
            OnDamage(actual, direction, knockback);

            if(health <= 0)
            {
                dead = true;
                OnKill(taken, direction);
            }
            return true;
        }
        OnDamage(0, direction, knockback);
        return false;
    }

    public bool Heal(float heal)
    {
        if(!damageable) return false;
        
        health += heal;
        OnHeal(heal);
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        return true;
    }

    public bool GainArmour(float amount)
    {
        if(!damageable) return false;

        armour += amount;
        OnGainArmour(amount);
        if(armour > maxArmour)
        {
            armour = maxArmour;
        }
        return true;
    }
}
