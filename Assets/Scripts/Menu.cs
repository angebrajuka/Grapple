using UnityEngine;

public class Menu : MonoBehaviour
{
    public bool escBack;

    public void Back()
    {
        MenuHandler.Back();
    }

    void Update()
    {
        if(escBack && Input.GetKeyDown(KeyCode.Escape))
        {
            MenuHandler.Back();
        }
    }
}