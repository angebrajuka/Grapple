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
    public float ammo;

    public GrappleHook hook;
    public float reloadStartTime;

    static float p_compressedAir;
    public static float CompressedAir
    {
        get { return p_compressedAir; }
        set
        {
            p_compressedAir = value;
            instance.CheckReload();
            PlayerHUD.UpdateCompressedAir();
        }
    }
    static float airPerShot;

    public void Init()
    {
        instance = this;
        CompressedAir = 1;
        airPerShot = 1f/ammo;
    }


    public static bool IsLoaded { get { return Time.time > instance.reloadStartTime + instance.reloadTime && instance.hook == null; } }
    public static bool IsReloading { get { return Time.time > instance.reloadStartTime && Time.time <= instance.reloadStartTime + instance.reloadTime; } }
    public static bool HasGas { get { return CompressedAir >= airPerShot-0.001f; } }
    public static bool IsGrappling { get { return instance.hook != null; } }
    public static bool CanShoot { get { return IsLoaded && HasGas; } }

    public void CheckReload()
    {
        if(!IsGrappling && HasGas && !IsLoaded && !IsReloading)
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
        hook.configJoint.connectedBody = PlayerMovement.m_rigidbody;
        rb.AddForce(direction*shootForce);
        PlayerMovement.m_rigidbody.AddRelativeForce(0, 0, -recoilForce);
        CompressedAir -= airPerShot;
        reloadStartTime = Mathf.Infinity;
    }

    void Update()
    {
        if(GetKey("grapple_shoot") && !IsGrappling && CanShoot)
        {
            ShootHook();
            AudioManager.PlayClip(clip_shoot, volume_shoot);
        }
        else if(GetKeyUp("grapple_shoot") && IsGrappling)
        {
            hook.Retract();
        }
        else if(IsGrappling && GetKeyDown("grapple_shoot"))
        {
            Destroy(hook.fixedJoint);
            hook.Retract();
        }
    }
}