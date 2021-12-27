using UnityEngine;
using UnityEngine.Pool;

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

    ObjectPool<Bullet> pool_bullets;

    void Start()
    {
        timeBetweenShots = 60f/rpm;
        ammo = 0;
        timeLastShot = 0;
        primed = true;
        pool_bullets = new ObjectPool<Bullet>(
            () => {
                // on create
                var bullet = Instantiate(prefab_projectile, transform).GetComponent<Bullet>();
                bullet.pool = pool_bullets;

                return bullet;
            },
            (bullet) => {
                // on get
                bullet.OnGet();
                bullet.gameObject.SetActive(true);
            },
            (bullet) => {
                // on return
                bullet.gameObject.SetActive(false);
            },
            (bullet) => {
                // on destroy
                Destroy(bullet.gameObject);
            },
            false, 50, 50
        );
    }

    void ShootBullet(int pellet)
    {
        float spread = this.spread * ((float)pellet / pellets);
        float horzSpread = Random.Range(-spread, spread);
        float vertSpread = spread-Mathf.Abs(horzSpread);
        var eulerOffset = new Vector3(Random.Range(-vertSpread, vertSpread), horzSpread, 0);

        var bullet = pool_bullets.Get();
        bullet.transform.SetPositionAndRotation(transform.position-transform.forward*0.5f, transform.rotation);
        bullet.transform.localEulerAngles += eulerOffset;
        bullet.transform.parent = null;

        bullet.rb.velocity = bullet.transform.forward*projectileForce;
        bullet.SetRange(range);
        bullet.rb.velocity += PlayerMovement.m_rigidbody.velocity*0.7f;
        bullet.damage = (float)damage / (float)pellets;
    }

    public void Shoot()
    {
        PlayerAnimator.instance.gunReloadAnimator.SetInteger("state", 0);
        AudioManager.PlayClip(clip_shoot);
        PlayerMovement.m_rigidbody.AddForce(PlayerMovement.instance.t_camera.TransformDirection(0, 0, -recoil));
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