using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    // hierarchy
    public bool grenade;
    public GameObject mesh;
    public GameObject trail;
    public Rigidbody rb;
    [Header("Grenade Specific")]
    public float detonationTime;
    public float randomRotation;
    public GameObject prefab_explosion;

    public float lifetime;
    public float damage;
    public float distancePerTick;
    public LayerMask layermask;
    // grenade specific
    bool firstBounce;

    float timeShot;

    public ObjectPool<GameObject> pool;

    public void SetRange(float range)
    {
        lifetime = range / rb.velocity.magnitude;
    }

    public void OnGet()
    {
        timeShot = Time.time;
        mesh.SetActive(false);
        if(trail != null) 
        {
            trail.GetComponent<TrailRenderer>().Clear();
            trail.SetActive(false);
        }

        if(grenade)
        {
            firstBounce = true;
            rb.AddTorque(new Vector3(Random.value, Random.value, Random.value)*randomRotation);
        }
    }

    void Release()
    {
        pool.Release(this.gameObject);
    }

    public void DetonateGrenade()
    {
        Instantiate(prefab_explosion, transform.position, transform.rotation, transform.parent);
    }

    void OnCollisionEnter(Collision c)
    {
        var target = c.gameObject.GetComponent<Target>();
        if(target != null)
        {
            if(!grenade)
            {
                target.Damage(damage, transform.forward, 50);
            }
            else if(firstBounce)
            {
                DetonateGrenade();
                Release();
            }
        }
        firstBounce = false;

        if(!grenade) Release();
    }

    void FixedUpdate()
    {
        mesh.SetActive(true);
        if(trail != null) trail.SetActive(true);
        if(Time.time >= timeShot + (grenade ? detonationTime : lifetime))
        {
            if(grenade) DetonateGrenade();
            Release();
        }
    }
}