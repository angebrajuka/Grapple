using UnityEngine;
using UnityEngine.UI;

public class MenuMouseSettings : MonoBehaviour
{
    public Slider sx, sy, ss;

    void OnEnable()
    {
        sx.value = PlayerInput.speed_look.x / PlayerInput.MAX_LOOK_SPEED;
        sy.value = PlayerInput.speed_look.y / PlayerInput.MAX_LOOK_SPEED;
        ss.value = PlayerInput.speed_scroll / PlayerInput.MAX_SCROLL_SPEED;
    }

    public void SliderXSenseChanged()
    {
        PlayerInput.speed_look.x = sx.value * PlayerInput.MAX_LOOK_SPEED;
        PlayerInput.SaveLookSpeed();
    }

    public void SliderYSenseChanged()
    {
        PlayerInput.speed_look.y = sy.value * PlayerInput.MAX_LOOK_SPEED;
        PlayerInput.SaveLookSpeed();
    }

    public void SliderScrollSenseChanged()
    {
        PlayerInput.speed_scroll = ss.value * PlayerInput.MAX_SCROLL_SPEED;
        PlayerInput.SaveScrollSpeed();
    }
}