using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseHandler : MonoBehaviour
{
    public static PauseHandler instance;

    // hierarchy
    public Volume volume;
    public float blurredFocalLength, unblurredFocalLength;
    public float blurSpeed, unblurSpeed;

    static DepthOfField dofComponent;
    public static bool paused;
    public static bool blurred;
    public static bool frozenInput;

    public void Init()
    {
        instance = this;

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
        blurred = true;
        PlayerHUD.instance.t_HUD.gameObject.SetActive(false);
        ShowCursor();

        paused = true;

        FreezePhysics();
        frozenInput = true;

        AudioManager.PauseAllAudio();
    }

    public static void UnPause()
    {
        blurred = false;
        PlayerHUD.instance.t_HUD.gameObject.SetActive(true);
        HideCursor();

        UnfreezePhysics();
        frozenInput = false;

        AudioManager.ResumeAllAudio();

        paused = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !paused && MenuHandler.CurrentMenu != 1)
        {
            Pause();
            MenuHandler.CurrentMenu = 0;
            return;
        }

        dofComponent.focalLength.value = Mathf.Lerp(dofComponent.focalLength.value, blurred ? blurredFocalLength : unblurredFocalLength, Time.unscaledDeltaTime*(blurred ? blurSpeed : unblurSpeed));
    }
}
