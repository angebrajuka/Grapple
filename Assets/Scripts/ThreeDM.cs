using UnityEngine;
using System.Collections.Generic;
using static PlayerInput;

public class ThreeDM : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_hook;
    public AudioClip clip_shoot;
    public float volume_shoot;
    public AudioSource source_cableSpinning;
    public float shootForce;
    public float autoRetractSpeedFast;
    public float autoRetractSpeedSlow;
    public float recoilForce;
    public float minDistance;
    public float destroyDistance;
    public float reloadTime;

    GrappleHook hook;
    public float returnTime;

    bool CanShoot
    {
        get { return Time.time - returnTime > reloadTime; }
    }

    public void ShootHook()
    {
        var obj = Instantiate(prefab_hook, transform.position, PlayerMovement.instance.t_camera.rotation, null);
        hook = obj.GetComponent<GrappleHook>();
        hook.threeDM = this;
        hook.configJoint.connectedBody = PlayerMovement.m_rigidbody;
        var rb = obj.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward*shootForce);
        rb.AddForce(PlayerMovement.m_rigidbody.velocity);
        PlayerMovement.m_rigidbody.AddRelativeForce(0, 0, -recoilForce);
    }

    void Update()
    {
        if(GetKey("grapple_shoot") && hook == null && CanShoot)
        {
            ShootHook();
            AudioManager.PlayClip(clip_shoot, volume_shoot);
        }
        else if(GetKeyUp("grapple_shoot") && hook != null)
        {
            hook.Retract();
        }
        else if(hook != null && GetKeyDown("grapple_shoot"))
        {
            Destroy(hook.fixedJoint);
            hook.Retract();
        }
    }
}