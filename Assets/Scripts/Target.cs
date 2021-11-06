using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public bool damageable;
    public bool dead;

    public void OnDamage(float damage, Vector3 direction)
    {
        var rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.AddForce(direction*damage*50f);
        }
    }

    public void OnKill(float damage, Vector3 direction)
    {
        Destroy(gameObject);
    }

    public void OnHeal(float health)
    {

    }

    public bool Damage(float damage, Vector3 direction=default(Vector3))
    {
        if(damageable && !dead)
        {
            health -= damage;
            OnDamage(damage, direction);

            if(health <= 0)
            {
                dead = true;
                OnKill(damage, direction);
            }
            return true;
        }
        OnDamage(0, direction);
        return false;
    }

    public bool Heal(float heal)
    {
        if(damageable)
        {
            health += heal;
            OnHeal(heal);
            if(health > maxHealth)
            {
                health = maxHealth;
                return true;
            }
        }
        return false;
    }
}
