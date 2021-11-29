using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    GameObject prefab;
    Transform parent;
    Queue<GameObject> pool;

    public ObjectPool(GameObject prefab, Transform parent=null)
    {
        pool = new Queue<GameObject>();
        this.prefab = prefab;
        this.parent = parent;
    }

    private GameObject DeQueue()
    {
        pool.Peek().SetActive(true);
        return pool.Dequeue();
    }

    private GameObject DeQueue(Vector3 position, Quaternion rotation)
    {
        pool.Peek().transform.SetPositionAndRotation(position, rotation);
        pool.Peek().SetActive(true);
        return pool.Dequeue();
    }

    public GameObject Get()
    {
        return pool.Count == 0 ? MonoBehaviour.Instantiate(prefab, parent) : DeQueue();
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        return pool.Count == 0 ? MonoBehaviour.Instantiate(prefab, position, rotation, parent) : DeQueue(position, rotation);
    }

    public void Return(GameObject gameObject)
    {
        gameObject.SetActive(false);
        pool.Enqueue(gameObject);
    }
}