using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
    public Transform hand;
    public AudioClip[] sounds;
    public float[] volumes;

    public void Sound(int index)
    {
        AudioManager.PlayClip(sounds[index], volumes[index]);
    }

    public void Finish()
    {
        PlayerInventory.Ammo = PlayerInventory.CurrentGunStats.magSize;
    }

    public void Update()
    {
        // playerHand.position = hand.localPostion;
        // playerHand.localEulerAngles = hand.localEulerAngles;
    }
}