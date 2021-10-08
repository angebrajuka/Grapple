using UnityEngine;

public class Init : MonoBehaviour
{
    // hierarchy
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

        // player.GetComponent<DynamicLoading>().Init();
        player.GetComponent<PlayerInput>().Init();
        player.GetComponent<PlayerMovement>().Init();
        // player.GetComponent<EnemySpawning>().Init();

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