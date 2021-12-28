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

    public void OnDamage(float damage, Vector3 direction, float knockback)
    {
        var rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.AddForce(direction*damage*knockback);
        }
    }

    public void OnKill(float damage, Vector3 direction)
    {
        Destroy(gameObject);
    }

    public void OnHeal(float health)
    {

    }

    public bool Damage(float damage, Vector3 direction=default(Vector3), float knockback=0)
    {
        if(damageable && !dead)
        {
            health -= damage;
            OnDamage(damage, direction, knockback);

            if(health <= 0)
            {
                dead = true;
                OnKill(damage, direction);
            }
            return true;
        }
        OnDamage(0, direction, knockback);
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
