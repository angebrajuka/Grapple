using UnityEngine;

public class Menu : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MenuHandler.Back();
            if(!MenuHandler.anyMenu) PauseHandler.UnPause();
        }
    }
}