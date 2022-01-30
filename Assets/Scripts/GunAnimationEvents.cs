using UnityEngine;

public class GunAnimationEvents : MonoBehaviour
{
    public void Raised()
    {
        PlayerAnimator.SetState(PlayerAnimator.RAISED);
    }

    public void Lowered()
    {
        PlayerAnimator.instance.AtLowest();
    }
}