using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuHandler : MonoBehaviour
{
    static MenuHandler instance;

    // hierarchy
    public long menuSeed;
    public MenuSpin menuSpin;

    private static int currentMenu;
    private static Stack<int> prevMenu;
    public static bool anyMenu;
    public static bool mainMenu;

    public void Init()
    {
        instance = this;

        prevMenu = new Stack<int>();

        for(int i=0; i<instance.transform.childCount; i++)
        {
            instance.transform.GetChild(i).gameObject.SetActive(false);
        }
        anyMenu = false;
        mainMenu = false;
    }

    public static void Close()
    {
        instance.transform.GetChild(currentMenu).gameObject.SetActive(false);
        anyMenu = false;
    }

    public static void Back()
    {
        Close();
        if(prevMenu.Count != 0)
        {
            currentMenu = prevMenu.Pop();
            instance.transform.GetChild(currentMenu).gameObject.SetActive(true);
            anyMenu = true;
        }
        else
        {
            PauseHandler.UnPause();
        }
    }

    public static void Save()
    {
        SaveData.Save();
    }

    public static void MainMenu(bool save=true)
    {
        if(save) {
            Save();
            SaveData.currentSaveFileName = "";
            SaveData.currentSaveName = "";
        }

        mainMenu = true;
        instance.menuSpin.Init();
        prevMenu.Clear();
        CurrentMenu = 1;
        PauseHandler.blurred = false;
        ProceduralGeneration.seed = instance.menuSeed;
        ProceduralGeneration.UnloadAll();

        PlayerAnimator.Overlay = false;
    }

    public static void StartGame()
    {
        mainMenu = false;
        PlayerAnimator.Overlay = true;
        ProceduralGeneration.UnloadAll();
        prevMenu.Clear();
        Close();
        PauseHandler.UnPause();
        Save();
    }

    public static int CurrentMenu
    {
        get
        {
            return currentMenu;
        }
        set
        {
            if(anyMenu)
            {
                if(currentMenu == value) return;

                prevMenu.Push(currentMenu);
                Close();
            }
            currentMenu = value;
            instance.transform.GetChild(currentMenu).gameObject.SetActive(true);
            anyMenu = true;
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
