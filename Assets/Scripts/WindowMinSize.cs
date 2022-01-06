using UnityEngine;

public class WindowMinSize : MonoBehaviour
{
    public int minWidth, minHeight;

    void Update()
    {
        if(Screen.width < minWidth || Screen.height < minHeight)
        {
            Screen.SetResolution(Mathf.Max(Screen.width, minWidth), Mathf.Max(Screen.height, minHeight), false);
        }
    }
}