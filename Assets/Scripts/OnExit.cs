using UnityEngine;

public class OnExit : MonoBehaviour
{
    void OnApplicationQuit()
    {
        if(SaveData.currentSaveFileName != "")
        {
            SaveData.Save();
        }
    }
}