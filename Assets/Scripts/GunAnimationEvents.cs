using UnityEngine;

public class GunAnimationEvents : MonoBehaviour
{
    public void Raised()
    {
        PlayerAnimator.state = PlayerAnimator.State.RAISED;
        if(PlayerInventory.Ammo <= 0) // TODO
        {
            PlayerAnimator.ActiveGun.gameObject.GetComponent<Animation>().Play();
        }
    }

    public void Lowered()
    {
        PlayerAnimator.instance.AtLowest();
    }
}