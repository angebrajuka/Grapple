using UnityEngine;

public class PlayerTarget : Target
{
    public static PlayerTarget instance;

    public void Init()
    {
        instance = this;
    }

    public override void OnDamage(float damage, Vector3 direction, float knockback)
    {
        Knockback(damage, direction, knockback);
        PlayerHUD.UpdateHealth();
        PlayerHUD.UpdateArmour();
    }

    public override void OnHeal(float health)
    {
        PlayerHUD.UpdateHealth();
    }

    public override void OnGainArmour(float amount)
    {
        PlayerHUD.UpdateArmour();
    }

    public override void OnKill(float damage, Vector3 direction)
    {

    }
}