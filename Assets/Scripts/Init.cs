using UnityEngine;

public class Init : MonoBehaviour
{
    // hierarchy
    public Guns guns;
    public Transform player;
    public Transform canvas;
    public AudioManager audioManager;
    public MusicController musicController;
    public MenuHandler menuHandler;
    public bool load;

    void Start()
    {
        audioManager.Init();
        musicController.Init();
        player.GetComponent<PauseHandler>().Init();

        guns.Init();
        PlayerInventory.Init();

        player.GetComponent<PlayerAnimator>().Init();
        player.GetComponent<PlayerInput>().Init();
        player.GetComponent<PlayerMovement>().Init();
        player.GetComponent<PlayerThreeDM>().Init();

        canvas.GetComponent<PlayerBloodUI>().Init();

        menuHandler.Init();

        if(load)
        {
            SaveData.TryLoad();
        }

        PauseHandler.UnPause();

        Destroy(gameObject);
    }
}