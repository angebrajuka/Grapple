using UnityEngine;
using System.Collections.Generic;

public class Guns : MonoBehaviour
{
    public static Dictionary<string, GunStats> guns;

    public void Init()
    {
        guns = new Dictionary<string, GunStats>();

        int index = 0;

        for(int i=0; i<transform.childCount; i++)
        {
            var gun = transform.GetChild(i).GetComponent<GunStats>();
            if(gun == null) continue;
            // gun.clip_shoot = Resources.Load<AudioClip>("clip_"+gun.name+"_shoot");
            // Debug.Assert(gun.clip_shoot != null, "clip_shoot null");
            // gun.prefab_muzzleFlash = Resources.Load<GameObject>("p_muzzleFlash_"+(gun.muzzleFlashName == "" ? gun.ammoType : gun.muzzleFlashName));
            // Debug.Assert(gun.prefab_muzzleFlash != null, "prefab_muzzleFlash null");
            // gun.mesh = transform.Find("mesh_"+gun.name);
            // Debug.Assert(gun.mesh != null, "mesh null");

            // gun.mesh.gameObject.SetActive(false);

            // gun.vec_barrelTip = new Vector3(gun.pos_barrelTip[0], gun.pos_barrelTip[1], gun.pos_barrelTip[2]);

            gun.index = index++;

            guns.Add(gun._name, gun);
        }

        guns.Add("", null);
    }
}