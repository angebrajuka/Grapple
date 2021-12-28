using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseHandler : MonoBehaviour
{
    public Volume volume;
    static DepthOfField dofComponent;
    static int focalLengthVal=1;
    public static bool paused;
    public static bool frozenInput;

    public void Init()
    {
        volume.profile.TryGet<DepthOfField>(out dofComponent);
        paused = false;
        frozenInput = false;
    }

    public static void FreezePhysics()
    {
        Time.timeScale = 0;
    }

    public static void UnfreezePhysics()
    {
        Time.timeScale = 1;
    }

    public static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void Pause()
    {
        Blur();
        ShowCursor();

        paused = true;

        FreezePhysics();
        frozenInput = true;

        AudioManager.PauseAllAudio();
    }

    public static void UnPause()
    {
        UnBlur();
        HideCursor();

        UnfreezePhysics();
        frozenInput = false;

        AudioManager.ResumeAllAudio();

        paused = false;
    }

    public static void Blur()
    {
        focalLengthVal = 100;
        dofComponent.focalLength.value = 15;
    }

    public static void UnBlur()
    {
        focalLengthVal = 1;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !paused)
        {
            Pause();
            MenuHandler.CurrentMenu = 0;
            return;
        }

        dofComponent.focalLength.value = Mathf.Lerp(dofComponent.focalLength.value, focalLengthVal, Time.unscaledDeltaTime*4);
    }
}
