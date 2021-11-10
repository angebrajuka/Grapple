using UnityEngine;

public class DetachChildrenAndDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}