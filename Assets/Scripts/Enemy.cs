using UnityEngine;

public class Enemy : MonoBehaviour
{
    // hierarchy
    public OnScreen onScreen;
    public float despawnDistance;

    float Distance
    {
        get { return Vector3.Distance(transform.position, PlayerMovement.m_rigidbody.position); }
    }

    public bool LineOfSight()
    {
        return false;
        // return !Physics.Raycast(transform.position, PlayerMovement.instance.t_camera.position-transform.position, Distance);
    }

    public bool IsOnScreen
    {
        get { return onScreen.onScreen; }
    }

    public void OnUpdate()
    {
        if(!onScreen.onScreen && Distance > despawnDistance)
        {
            MonoBehaviour.Destroy(transform.gameObject);
        }
    }
}