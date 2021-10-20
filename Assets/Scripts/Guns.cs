using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Gun
{
    public string       name;
    public string       ammoType;
    public int          damage;
    public int          rpm;
    public int          magSize;
    public int          ammoPerShot;
    public int          pellets;
    public int          range;
    public float        spread;
    public float        recoil;
    public float        reloadTime;
    public float        vol_shoot;
    public float        vol_reload;
    public float[]      pos_barrelTip; // relative to 0,0,0 in the .fbx file
    public string       muzzleFlashName;

    [System.NonSerialized()] public AudioClip    clip_shoot;
    [System.NonSerialized()] public Transform    mesh;
    [System.NonSerialized()] public GameObject   prefab_muzzleFlash;
    [System.NonSerialized()] public Vector3      vec_barrelTip;
}

public class GunsJson
{
    public Gun[] guns;
}

public class Guns : MonoBehaviour
{
    public static Dictionary<string, Gun> guns;

    // static void SetLayer(Transform transform, int depth)
    // {
    //     transform.gameObject.layer = Layers.PLAYER_ARMS;
        
    //     if(depth <= 0) return;
        
    //     for(int i=0; i<transform.childCount; i++)
    //     {
    //         SetLayer(transform.GetChild(i), depth-1);
    //     }
    // }

    public void Init()
    {
        guns = new Dictionary<string, Gun>();

        var gunsJson = JsonUtility.FromJson<GunsJson>(Resources.Load<TextAsset>("Guns").text);
        foreach(var gun in gunsJson.guns)
        {
            gun.clip_shoot = Resources.Load<AudioClip>("clip_"+gun.name+"_shoot");
            Debug.Assert(gun.clip_shoot != null, "clip_shoot null");
            gun.prefab_muzzleFlash = Resources.Load<GameObject>("p_muzzleFlash_"+(gun.muzzleFlashName == "" ? gun.ammoType : gun.muzzleFlashName));
            Debug.Assert(gun.prefab_muzzleFlash != null, "prefab_muzzleFlash null");
            gun.mesh = transform.Find("mesh_"+gun.name);
            Debug.Assert(gun.mesh != null, "mesh null");

            gun.mesh.gameObject.SetActive(false);

            gun.vec_barrelTip = new Vector3(gun.pos_barrelTip[0], gun.pos_barrelTip[1], gun.pos_barrelTip[2]);

            guns.Add(gun.name, gun);
        }

        guns.Add("", null);
    }
}