using UnityEngine;

public class DestroyWhenDonePlaying : MonoBehaviour
{
    public AudioSource s;
    public bool paused;

    void Update()
    {
        if(!s.isPlaying && !paused) Destroy(gameObject);
    }
}