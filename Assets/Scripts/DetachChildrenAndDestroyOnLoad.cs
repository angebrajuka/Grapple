using UnityEngine;

public class DetachChildrenAndDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(0).parent = null;
        }
        Destroy(gameObject);
    }
}