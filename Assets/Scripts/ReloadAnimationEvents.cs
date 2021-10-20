using UnityEngine;

public class ReloadAnimationEvents : MonoBehaviour
{
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
}