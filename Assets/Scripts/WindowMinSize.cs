using UnityEngine;

public class WindowMinSize : MonoBehaviour
{
    public int minWidth, minHeight;
    public float minAspect, maxAspect;

    int prevWidth=0, prevHeight=0;

    void Update()
    {
        if(Screen.width == prevWidth && Screen.height == prevHeight) return;

        if(Screen.width < minWidth || Screen.height < minHeight)
        {
            Screen.SetResolution(Mathf.Max(Screen.width, minWidth), Mathf.Max(Screen.height, minHeight), false);
        }

        if((float)Screen.width/(float)Screen.height > maxAspect)
        {
            Screen.SetResolution((int)Mathf.Floor(Screen.height * maxAspect), Screen.height, false);
        }
        else if((float)Screen.width/(float)Screen.height < minAspect)
        {
            Screen.SetResolution(Screen.width, (int)Mathf.Floor(Screen.width / minAspect), false);
        }
    }
}