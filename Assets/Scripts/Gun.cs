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
        var directionOffset = Vector3.zero;

        if(prefab_projectile != null)
        {
            var go = Instantiate(prefab_projectile, transform.position+(transform.forward*barrelLength), Quaternion.identity, null);
            var rb = go.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward*projectileForce);
            rb.AddTorque(Random.onUnitSphere*(projectileForce/50f));
        }
        else
        {
            RaycastHit hit;
            if(PlayerEyes.Raycast(out hit, range, barrelLength*Vector3.forward, directionOffset))
            {
                
            }
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