using UnityEngine;

public class Gun : MonoBehaviour
{
    public string       _name;
    public string       ammoType;
    public float        damage;
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
    public GameObject   prefab_muzzleFlash;
    public GameObject   prefab_projectile;
    public float        projectileForce;

    [HideInInspector] public int index;
    [HideInInspector] public float timeBetweenShots;
    [HideInInspector] public int ammo;
    [HideInInspector] public float timeLastShot;
    [HideInInspector] public bool primed;

    ObjectPool pool_bullets;

    void Start()
    {
        timeBetweenShots = 60f/rpm;
        ammo = 0;
        timeLastShot = 0;
        primed = true;
        pool_bullets = new ObjectPool(prefab_projectile, null);
    }

    void ShootBullet(int pellet)
    {
        float spread = this.spread * ((float)pellet / pellets);
        float horzSpread = Random.Range(-spread, spread);
        float vertSpread = spread-Mathf.Abs(horzSpread);
        var eulerOffset = new Vector3(Random.Range(-vertSpread, vertSpread), horzSpread, 0);

        var go = pool_bullets.Get(transform.position-transform.forward*0.5f, transform.rotation);
        go.transform.eulerAngles += eulerOffset;
        var rb = go.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity += PlayerMovement.m_rigidbody.velocity*0.7f;
            rb.AddForce(transform.forward*projectileForce);
        }
        var bullet = go.GetComponent<Bullet>();
        if(bullet != null)
        {
            bullet.pool = pool_bullets;
            bullet.range = range;
            bullet.damage = (float)damage / (float)pellets;
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
            ShootBullet(i);
        }
    }
}