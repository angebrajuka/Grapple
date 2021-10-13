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
    [HideInInspector] public float returnTime;

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
        if(GetKeyDown("grapple_shoot"))
        {
            if(hook == null && CanShoot)
            {
                ShootHook();
                AudioManager.PlayClip(clip_shoot, volume_shoot);
            }
            else
            {
                hook.Retract();
            }
        }
        if(hook != null)
        {
            if(GetKeyDown("grapple_end") && hook.configJoint.connectedMassScale != 0)
            {
                hook.GetComponent<Collider>().enabled = false;
                hook.Retract();
                hook.configJoint.connectedMassScale = 0;
                Destroy(hook.fixedJoint);
            }
        }
    }
}