using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIAmmoGraphic : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_ammoTick;
    public float scale;

    int mag=0, ammo=0;
    Ammo ammoType;
    List<RectTransform> ticks;
    Dictionary<Ammo, float> spacings;
    public Color color;

    void Start() {
        spacings = new Dictionary<Ammo, float>();
        foreach(var ammoData in PlayerInventory.instance.ammoDatas) {
            spacings.Add(ammoData.ammo, ammoData.spacing);
        }
        ticks = new List<RectTransform>();
    }

    RawImage Image(Transform t)
    {
        return t.GetChild(0).GetComponent<RawImage>();
    }

    void Update()
    {
        Ammo pAmmoType = PlayerInventory.CurrentGun.ammoType;
        int pMag = PlayerInventory.CurrentGun.magSize;
        if(pAmmoType != ammoType || pMag != mag)
        {
            ammoType = pAmmoType;
            for(int i=0; i<50 && ticks.Count>0; i++) {
                Destroy(ticks[0].gameObject);
                ticks.RemoveAt(0);
            }

            mag = pMag;
            for(int i=0; i<mag; i++) {
                var go = Instantiate(prefab_ammoTick, transform);
                var rect = go.GetComponent<RectTransform>();
                rect.anchoredPosition += Vector2.left*spacings[ammoType]*i;
                ticks.Add(rect);

                for(int c=0; c<2; c++) {
                    var ri = go.transform.GetChild(c).GetComponent<RawImage>();
                    ri.texture = PlayerHUD.ammoImages[ammoType];
                    var col = ri.color;
                    for(var rgb=0; rgb<3; rgb++) {
                        col[rgb] = color[rgb];
                    }
                    ri.color = col;
                }
            }
            ammo = -1;
        }

        int pAmmo = PlayerInventory.Ammo;
        if(pAmmo != ammo)
        {
            ammo = pAmmo;
            for(int i=0; i<ticks.Count; i++)
            {
                ticks[i].GetChild(1).gameObject.SetActive(false);
            }
            for(int i=0; i<ammo; i++)
            {
                ticks[i].GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}