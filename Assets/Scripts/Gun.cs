using UnityEngine;
using UnityEngine.Pool;

public enum Ammo : byte {
    NATO556,
    SHELLS,
    GRENADES,
    PLASMA
}

public class Gun : MonoBehaviour
{
    public string       _name;
    public Ammo         ammoType;
    public float        damage;
    public int          rpm;
    public Chamber      chamberPostShot;
    public Chamber      chamberPostReload;
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
    public Transform    barrelTip;
    public GameObject   prefab_projectile;
    public float        projectileForce;

    [HideInInspector] public int index;
    [HideInInspector] public float timeBetweenShots;
    [HideInInspector] public int ammo;
    [HideInInspector] public float timeLastShot;
    public enum Chamber { EMPTY, SHELL, FULL }
    public Chamber chamber;

    ObjectPool<GameObject> pool_bullets;

    void Start()
    {
        timeBetweenShots = 60f/rpm;
        ammo = 0;
        timeLastShot = 0;
        pool_bullets = new ObjectPool<GameObject>(
            () => {
                // on create
                var bullet = Instantiate(prefab_projectile, transform).GetComponent<Bullet>();
                bullet.pool = pool_bullets;

                return bullet.gameObject;
            },
            (bullet) => {
                // on get
                bullet.SetActive(true);
                bullet.GetComponent<Bullet>().OnGet();
            },
            (bullet) => {
                // on return
                bullet.SetActive(false);
            },
            (bullet) => {
                // on destroy
                Destroy(bullet);
            },
            false, pellets*2, pellets*2
        );
    }

    void ShootBullet(int pellet)
    {
        float spread = this.spread * ((float)pellet / pellets);
        float horzSpread = Random.Range(-spread, spread);
        float vertSpread = spread-Mathf.Abs(horzSpread);
        var eulerOffset = new Vector3(Random.Range(-vertSpread, vertSpread), horzSpread, 0);

        var bullet = pool_bullets.Get().GetComponent<Bullet>();
        bullet.transform.SetPositionAndRotation(transform.position-transform.forward*0.5f, transform.rotation);
        bullet.transform.localEulerAngles += eulerOffset;
        bullet.transform.parent = null;

        bullet.rb.velocity = bullet.transform.forward*projectileForce;
        bullet.SetRange(range);
        // bullet.rb.velocity += PlayerMovement.rb.velocity*0.7f;
        bullet.damage = (float)damage / (float)pellets;
    }

    public void Shoot()
    {
        PlayerAnimator.SetState(PlayerAnimator.RECOIL_BACK);
        AudioManager.PlayClip(clip_shoot);
        PlayerMovement.rb.AddForce(PlayerMovement.instance.t_camera.TransformDirection(0, 0, -recoil));
        timeLastShot = Time.time;
        ammo -= ammoPerShot;
        chamber = chamberPostShot;

        if(prefab_muzzleFlash != null) Instantiate(prefab_muzzleFlash, barrelTip.position, transform.rotation, barrelTip);

        for(int i=0; i<pellets; i++)
        {
            ShootBullet(i);
        }
    }
}