using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;

    // hierarchy
    public Transform t_HUD;
    public RawImage crosshair_image;
    public Color crosshair_colorReady;
    public Color crosshair_colorReloading;
    public Text ammo_reserveText;
    public Transform compressedAir_needle;
    public Transform grappleRecharge_needle;
    public float compressedAir_minAngle, compressedAir_maxAngle;
    public float speed_grappleRechargeDecrease, speed_grappleRechargeIncrease, compressedAir_speed;
    public Slider slider_health;
    public Slider slider_armour;

    public static Dictionary<Ammo, Texture2D> ammoImages;
    float grappleRechargeAngle;
    float compressedAirAngle;  // store angles because of Unity auto convert to positive, fucks up my logic
    float compressedAirTarget;

    public void Init()
    {
        instance = this;

        ammoImages = new Dictionary<Ammo, Texture2D>();
        foreach(var image in PlayerInventory.instance.ammoDatas)
        {
            ammoImages.Add(image.ammo, image.tex);
        }

        compressedAirAngle = compressedAir_needle.localEulerAngles.z;
        compressedAirTarget = compressedAirAngle;
        grappleRechargeAngle = grappleRecharge_needle.localEulerAngles.z;
    }

    public static void UpdateAmmoReserve()
    {
        instance.ammo_reserveText.SetText(PlayerInventory.ReserveAmmo+"");
    }

    public static void UpdateCompressedAir()
    {
        instance.compressedAirTarget = Math.Remap(PlayerThreeDM.CompressedAir, 0, 1, instance.compressedAir_minAngle, instance.compressedAir_maxAngle);
    }

    public static void UpdateHealth()
    {
        instance.slider_health.value = PlayerTarget.instance.health / PlayerTarget.instance.maxHealth;
    }

    public static void UpdateArmour()
    {
        instance.slider_armour.value = PlayerTarget.instance.armour / PlayerTarget.instance.maxArmour;
    }


    void Update()
    {
        // compressedAir
        var eulers = instance.compressedAir_needle.localEulerAngles;
        compressedAirAngle = Mathf.Lerp(compressedAirAngle, compressedAirTarget, Time.deltaTime*compressedAir_speed);
        eulers.z = compressedAirAngle;
        instance.compressedAir_needle.localEulerAngles = eulers;

        // grapple recharge
        eulers = instance.grappleRecharge_needle.localEulerAngles;
        grappleRechargeAngle = (PlayerThreeDM.IsReloading || PlayerThreeDM.IsLoaded ? 
            Mathf.Lerp(grappleRechargeAngle, instance.compressedAir_maxAngle, Time.deltaTime*speed_grappleRechargeIncrease) : 
            Mathf.Lerp(grappleRechargeAngle, instance.compressedAir_minAngle, Time.deltaTime*speed_grappleRechargeDecrease));
        eulers.z = grappleRechargeAngle;
        instance.grappleRecharge_needle.localEulerAngles = eulers;
    }
}