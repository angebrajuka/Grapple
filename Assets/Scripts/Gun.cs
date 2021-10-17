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
    public float[]      pos_barrelTip;
    public string       muzzleFlashName;

    [System.NonSerialized()] public AudioClip    clip_shoot;
    [System.NonSerialized()] public AudioClip    clip_reload;
    [System.NonSerialized()] public Mesh         mesh;
    [System.NonSerialized()] public GameObject   prefab_muzzleFlash;
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
            gun.prefab_muzzleFlash = Resources.Load<GameObject>("prefab_muzzleFlash_/"+gun.muzzleFlashName == null ? gun.ammoType : gun.muzzleFlashName);
            gun.mesh = Resources.Load<Mesh>("mesh_"+gun.name);

            guns.Add(gun.name, gun);
        }
    }
}