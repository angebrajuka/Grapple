using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;

    // hierarchy
    public Transform t_HUD;
    public RawImage crosshair_image;
    public Color crosshair_colorReady;
    public Color crosshair_colorReloading;
    public TextMeshProUGUI ammo_reserveText;
    public Slider slider_health;
    public Slider slider_armour;

    public static Dictionary<Ammo, Texture2D> ammoImages;

    public void Init()
    {
        instance = this;

        ammoImages = new Dictionary<Ammo, Texture2D>();
        foreach(var image in PlayerInventory.instance.ammoDatas)
        {
            ammoImages.Add(image.ammo, image.tex);
        }
    }

    public static void UpdateAmmoReserve()
    {
        instance.ammo_reserveText.SetText(PlayerInventory.ReserveAmmo+"");
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
        
    }
}