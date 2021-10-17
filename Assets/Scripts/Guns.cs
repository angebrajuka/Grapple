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
    public int          bulletsPerShot;
    public int          pellets;
    public int          range;
    public float        spread;
    public float        recoil;
    public float        reloadTime;
    public float        vol_shoot;
    public float        vol_reload;
    public float[]      pos_offset;
    public float[]      pos_barrelTip; // relative to 0,0,0 in the .fbx file
    public string       muzzleFlashName;

    [System.NonSerialized()] public AudioClip    clip_shoot;
    [System.NonSerialized()] public AudioClip    clip_reload;
    [System.NonSerialized()] public Mesh         mesh;
    [System.NonSerialized()] public GameObject   prefab_muzzleFlash;
    [System.NonSerialized()] public Vector3      vec_offset;
    [System.NonSerialized()] public Vector3      vec_barrelTip;
}

public class GunsJson
{
    public Gun[] guns;
}

public class Guns
{
    public static Dictionary<string, Gun> guns;

    public static void Init()
    {
        guns = new Dictionary<string, Gun>();

        var gunsJson = JsonUtility.FromJson<GunsJson>(Resources.Load<TextAsset>("Guns").text);
        foreach(var gun in gunsJson.guns)
        {
            gun.clip_shoot = Resources.Load<AudioClip>("clip_"+gun.name+"_shoot");
            gun.clip_reload = Resources.Load<AudioClip>("clip_"+gun.name+"_reload");
            gun.prefab_muzzleFlash = Resources.Load<GameObject>("p_muzzleFlash_"+(gun.muzzleFlashName == "" ? gun.ammoType : gun.muzzleFlashName));
            gun.mesh = Resources.Load<Mesh>("mesh_"+gun.name);

            Debug.Assert(gun.clip_shoot != null, "clip_shoot null");
            Debug.Assert(gun.clip_reload != null, "clip_reload null");
            Debug.Assert(gun.prefab_muzzleFlash != null, "prefab_muzzleFlash null");
            Debug.Assert(gun.mesh != null, "mesh null");

            gun.vec_barrelTip = new Vector3(gun.pos_barrelTip[0], gun.pos_barrelTip[1], gun.pos_barrelTip[2]);
            gun.vec_offset = new Vector3(gun.pos_offset[0], gun.pos_offset[1], gun.pos_offset[2]);

            guns.Add(gun.name, gun);
        }
    }
}