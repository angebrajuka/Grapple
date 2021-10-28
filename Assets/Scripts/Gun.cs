using UnityEngine;

public class Gun : MonoBehaviour
{
    public string       _name;
    public string       ammoType;
    public int          damage;
    public int          rpm;
    public bool         animateBetweenShots;
    public bool         animatePostReload;
    public int          magSize;
    public bool         shotgunReload;
    public int          ammoPerShot;
    public int          pellets;
    public int          range;
    public float        spread;
    public float        recoil;
    public AudioClip    clip_shoot;
    public float        vol_shoot; 
    public AudioClip[]  clip_reloads;
    public float[]      volume_reloads;
    public float        barrelLength; // relative to 0,0,0 in the .fbx file, which is always in line with the barrel, hence only need length
    public GameObject   prefab_muzzleFlash;

    [HideInInspector] public int index;
    [HideInInspector] public float timeBetweenShots;
    [HideInInspector] public int ammo;
    [HideInInspector] public float timeLastShot=0;
    [HideInInspector] public bool primed=false;

    void Start()
    {
        timeBetweenShots = 60f/rpm;
    }
}