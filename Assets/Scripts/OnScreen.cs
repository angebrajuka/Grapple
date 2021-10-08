using UnityEngine;

public class OnScreen : MonoBehaviour
{
    public bool onScreen;

    void OnBecameVisible()
    {
        onScreen = true;
    }

    void OnBecameInvisible()
    {
        onScreen = false;
    }
}