using UnityEngine;

public class GunAnimationEvents : MonoBehaviour
{
    public void Raised()
    {
        PlayerAnimator.state = PlayerAnimator.State.RAISED;
        PlayerAnimator.GunPosAnimator.enabled = false;
    }

    public void Lowered()
    {
        PlayerAnimator.instance.AtLowest();
    }
}