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

    Dictionary<string, Texture2D> dict_ammoImages;

    public void Init()
    {
        instance = this;

        dict_ammoImages = new Dictionary<string, Texture2D>();
        foreach(var pair in ammoImages)
        {
            dict_ammoImages.Add(pair.ammoType, pair.ammoImage);
        }
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
}