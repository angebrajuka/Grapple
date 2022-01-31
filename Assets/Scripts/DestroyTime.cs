using UnityEngine;

public class DestroyTime : MonoBehaviour {
    float time;

    void Start() {
        Destroy(gameObject, time);
    }
}