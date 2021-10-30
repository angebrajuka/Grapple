using UnityEngine;
using UnityEngine.UI;

public class UIAmmoGraphic : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_ammoTick;
    public bool mag; // uses magSize when true, playerAmmo when false
    public float spacing;
    public Color color;
    public float scale;

    int ammo=0;
    string ammoType;
    Transform lowest;

    RawImage Image(Transform t)
    {
        return t.GetChild(0).GetComponent<RawImage>();
    }

    void Update()
    {
        string pAmmoType = PlayerInventory.CurrentGun.ammoType;
        if(pAmmoType != ammoType)
        {
            ammoType = pAmmoType;
            Transform current = lowest;
            for(int i=0; i<50 && current != null && current != transform; i++)
            {
                var image = Image(current);
                image.texture = PlayerHUD.ammoImages[ammoType];
                current = current.parent;
            }
        }

        int pAmmo = mag ? PlayerInventory.CurrentGun.magSize : PlayerInventory.Ammo;
        if(pAmmo != ammo)
        {
            for(int i=0; i<ammo-pAmmo; i++)
            {
                Destroy(lowest.gameObject);
                lowest = lowest.parent;
            }
            for(int i=0; i<pAmmo-ammo; i++)
            {
                if(lowest == null) lowest = transform;
                var go = Instantiate(prefab_ammoTick, lowest);
                var rect = go.GetComponent<RectTransform>();
                var pos = rect.anchoredPosition;
                pos.Set(0, 0);
                rect.anchoredPosition = pos;
                var image = Image(go.transform);
                image.color = color;
                image.texture = PlayerHUD.ammoImages[ammoType];
                lowest = go.transform;
            }
            ammo = pAmmo;
        }
    }
}