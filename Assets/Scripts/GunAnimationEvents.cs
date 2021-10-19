using UnityEngine;

public class GunAnimationEvents : MonoBehaviour
{
    public void Raised()
    {
        PlayerAnimator.state = PlayerAnimator.State.RAISED;
        if(true) // TODO change to reload logic
        {
            PlayerAnimator.ActiveGun.gameObject.GetComponent<Animation>().Play();
        }
    }

    public void Lowered()
    {
        PlayerAnimator.instance.AtLowest();
    }
}