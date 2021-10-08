using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    // hierarchy
    // wow such empty

    static MenuHandler instance;
    private static int currentMenu;

    public void Init()
    {
        instance = this;

        for(int i=0; i<instance.transform.childCount; i++)
        {
            instance.transform.GetChild(i).gameObject.SetActive(false);
        }
        Close(true);
    }

    public static void Close(bool all)
    {
        instance.transform.GetChild(currentMenu).gameObject.SetActive(false);
        if(all)
        {
            // PlayerInput.instance.enabled = true;
        }
    }

    public static int CurrentMenu
    {
        get
        {
            return currentMenu;
        }
        set
        {
            Close(false);
            currentMenu = value;
            instance.transform.GetChild(currentMenu).gameObject.SetActive(true);
        }
    }
}
