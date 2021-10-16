using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    // hierarchy
    public Transform t_HUD;
    public RawImage crosshair;
    public Color crosshairColorReady;
    public Color crosshairColorReloading;

    void Update()
    {
        crosshair.color = PlayerThreeDM.instance.CanShoot ? crosshairColorReady : crosshairColorReloading;
    }
}