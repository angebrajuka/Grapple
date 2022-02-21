using UnityEngine;
using System.Collections.Generic;
using static PlayerInput;

public class PlayerThreeDM : MonoBehaviour
{
    public static PlayerThreeDM instance;

    // hierarchy
    public Transform t_threeDM;
    public GameObject prefab_hook;
    public AudioClip clip_shoot;
    public AudioClip clip_return;
    public AudioClip clip_reload;
    public float volume_shoot;
    public AudioSource source_cableSpinning;
    public float fixedJointBreakForce;
    public float shootForce;
    public float autoRetractSpeedFast;
    public float autoRetractSpeedSlow;
    public float recoilForce;
    public float minDistance;
    public float destroyDistance;
    public float reloadTime;

    public GrappleHook hook;
    public float reloadStartTime;

    public void Init()
    {
        instance = this;
    }


    public static bool IsLoaded { get { return Time.time > instance.reloadStartTime + instance.reloadTime && instance.hook == null; } }
    public static bool IsReloading { get { return Time.time > instance.reloadStartTime && Time.time <= instance.reloadStartTime + instance.reloadTime; } }
    public static bool IsGrappling { get { return instance.hook != null; } }
    public static bool CanShoot { get { return IsLoaded; } }

    public void CheckReload()
    {
        if(!IsGrappling && !IsLoaded && !IsReloading)
        {
            AudioManager.PlayClip(clip_reload);
            instance.reloadStartTime = Time.time;
        }
    }

    public void ShootHook()
    {
        var obj = Instantiate(prefab_hook, t_threeDM.position, PlayerMovement.instance.t_camera.rotation, null);
        RaycastHit hit;
        Vector3 direction = PlayerMovement.instance.t_camera.TransformDirection(Vector3.forward);
        var rb = obj.GetComponent<Rigidbody>();
        if(PlayerEyes.Raycast(out hit))
        {
            obj.transform.LookAt(hit.point);
            direction = (hit.point-rb.position).normalized;
        }
        hook = obj.GetComponent<GrappleHook>();
        hook.configJoint.connectedBody = PlayerMovement.rb;
        rb.AddForce(direction*shootForce);
        reloadStartTime = Mathf.Infinity;
    }

    void Update()
    {
        if(GetKey("shoot grapple hook") && !IsGrappling && CanShoot)
        {
            ShootHook();
            AudioManager.PlayClip(clip_shoot, volume_shoot);
        }
        else if(GetKeyUp("shoot grapple hook") && IsGrappling)
        {
            Destroy(hook.fixedJoint);
            hook.Retract();
        }
    }
}