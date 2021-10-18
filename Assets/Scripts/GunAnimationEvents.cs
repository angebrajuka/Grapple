using UnityEngine;

public class GunAnimationEvents : MonoBehaviour
{
    public void Raised()
    {
        PlayerAnimator.instance.state = PlayerAnimator.State.RAISED;
    }

    public void Lowered()
    {
        PlayerAnimator.instance.AtLowest();
    }
}