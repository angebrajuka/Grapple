using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;

    // hierarchy
    public Transform t_HUD;
    public RawImage crosshair_image;
    public Color crosshair_colorReady;
    public Color crosshair_colorReloading;
    public Text ammo_reserveText, ammo_magText;

    public void Init()
    {
        instance = this;
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