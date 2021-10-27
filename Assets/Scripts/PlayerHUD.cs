using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class StringTexturePair
{
    public string ammoType;
    public Texture2D ammoImage;
}

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;

    // hierarchy
    public Transform t_HUD;
    public RawImage crosshair_image;
    public Color crosshair_colorReady;
    public Color crosshair_colorReloading;
    public Text ammo_reserveText, ammo_magText;
    public RawImage ammoImage;
    public StringTexturePair[] ammoImages;
    public Transform compressedAir_needle;
    public Transform grappleRecharge_needle;
    public float compressedAir_minAngle, compressedAir_maxAngle;
    public float speed_grappleRechargeDecrease, speed_grappleRechargeIncrease, compressedAir_speed;

    Dictionary<string, Texture2D> dict_ammoImages;
    float grappleRechargeAngle;
    float compressedAirAngle;  // store angles because of Unity auto convert to positive, fucks up my logic
    float compressedAirTarget;

    public void Init()
    {
        instance = this;

        dict_ammoImages = new Dictionary<string, Texture2D>();
        foreach(var pair in ammoImages)
        {
            dict_ammoImages.Add(pair.ammoType, pair.ammoImage);
        }

        compressedAirAngle = compressedAir_needle.localEulerAngles.z;
        compressedAirTarget = compressedAirAngle;
        grappleRechargeAngle = grappleRecharge_needle.localEulerAngles.z;
    }

    public static void UpdateAmmoImage()
    {
        instance.ammoImage.texture = instance.dict_ammoImages[PlayerInventory.CurrentGun.ammoType];
    }
    public static void UpdateAmmoReserve()
    {
        instance.ammo_reserveText.SetText(PlayerInventory.ReserveAmmo+"");
    }
    public static void UpdateAmmoMag()
    {
        instance.ammo_magText.text = PlayerInventory.Ammo+"";
    }

    public static void UpdateCompressedAir()
    {
        instance.compressedAirTarget = Math.Remap(PlayerThreeDM.CompressedAir, 0, 1, instance.compressedAir_minAngle, instance.compressedAir_maxAngle);
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