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
    public float        barrelLength; // relative to 0,0,0 in the .fbx file, which is always in line with the barrel, hence only need length
    public GameObject   prefab_muzzleFlash;
    public GameObject   prefab_projectile;
    public float        projectileForce;

    [HideInInspector] public int index;
    [HideInInspector] public float timeBetweenShots;
    [HideInInspector] public int ammo;
    [HideInInspector] public float timeLastShot;
    [HideInInspector] public bool primed;

    void Start()
    {
        timeBetweenShots = 60f/rpm;
        ammo = 0;
        timeLastShot = 0;
        primed = true;
    }

    void ShootBullet()
    {
        float horzSpread = Random.Range(-spread, spread);
        float vertSpread = spread-horzSpread;
        var eulerOffset = new Vector3(Random.Range(-vertSpread, vertSpread), horzSpread, 0);

        var go = Instantiate(prefab_projectile, transform.position+(transform.forward*barrelLength), transform.rotation, null);
        go.transform.localEulerAngles += eulerOffset;
        var rb = go.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity += PlayerMovement.m_rigidbody.velocity*0.7f;
            rb.AddForce(transform.forward*projectileForce);
        }
        var bullet = go.GetComponent<Bullet>();
        if(bullet != null)
        {
            bullet.range = range;

        }
    }

    public void Shoot()
    {
        PlayerAnimator.instance.gunReloadAnimator.SetInteger("state", 0);
        AudioManager.PlayClip(clip_shoot);
        PlayerMovement.m_rigidbody.AddForce(PlayerMovement.instance.t_camera.TransformPoint(0, 0, -recoil));
        PlayerAnimator.instance.Recoil();
        timeLastShot = Time.time;
        ammo -= ammoPerShot;
        primed = false;

        for(int i=0; i<pellets; i++)
        {
            ShootBullet();
        }
    }
}