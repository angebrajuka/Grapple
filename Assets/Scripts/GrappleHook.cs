using UnityEngine;
using static GrappleHook.State;

public class GrappleHook : MonoBehaviour
{
    // hierarchy
    public AudioClip clip_reload;
    public ConfigurableJoint configJoint;
    public Rigidbody m_rigidbody;
    public LineRenderer m_lineRenderer;

    public FixedJoint fixedJoint;
    public ThreeDM threeDM;

    public enum State
    {
        SHOOTING,
        SWINGING,
        RETRACTING
    }
    public State state = SHOOTING;

    public void LockMotion(Rigidbody other)
    {
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = other;
    }

    public void Retract()
    {
        if(state == RETRACTING) return;

        state = RETRACTING;
        configJoint.SetDistance();
        if(fixedJoint == null)
        {
            GetComponent<Collider>().enabled = false;
            configJoint.connectedMassScale = 0;
        }
        threeDM.source_cableSpinning.Play();
    }

    void OnCollisionEnter(Collision other)
    {
        if(state != SHOOTING) return;

        LockMotion(other.rigidbody);
        configJoint.SetDistance();
        state = SWINGING;
    }

    void FixedUpdate()
    {
        if(state == RETRACTING)
        {
            if(configJoint.linearLimit.limit > threeDM.minDistance)
            {
                var ll = configJoint.linearLimit;
                ll.limit -= (fixedJoint == null ? threeDM.autoRetractSpeedFast : threeDM.autoRetractSpeedSlow);
                configJoint.linearLimit = ll;
                var quat = Quaternion.LookRotation(m_rigidbody.position-PlayerMovement.m_rigidbody.position);
                m_rigidbody.rotation = quat;
                // TODO  fix this patch mess of rotation
            }
            else
            {
                threeDM.source_cableSpinning.Stop();
            }
        }
    }

    void Update()
    {
        if(state == RETRACTING && fixedJoint == null && Vector3.Distance(configJoint.GetStart(), configJoint.GetEnd()) < threeDM.destroyDistance)
        {
            AudioManager.PlayClip(clip_reload);
            threeDM.returnTime = Time.time;
            threeDM.source_cableSpinning.Stop();
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        m_lineRenderer.SetPosition(0, m_rigidbody.position+configJoint.anchor);
        m_lineRenderer.SetPosition(m_lineRenderer.positionCount-1, threeDM.transform.position);
    }
}