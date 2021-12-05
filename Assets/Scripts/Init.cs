using UnityEngine;

public class Init : MonoBehaviour
{
    // hierarchy
    public AudioManager audioManager;
    public MusicController musicController;
    public Transform proceduralGeneration;
    public Transform player;
    public Guns guns;
    public EnemySpawning enemySpawning;
    public Transform canvas;
    public MenuHandler menuHandler;
    public bool load;

    void Start()
    {
        audioManager.Init();
        musicController.Init();
        proceduralGeneration.GetComponent<ProceduralGeneration>().Init();

        player.GetComponent<PauseHandler>().Init();

        guns.Init();
        player.GetComponent<PlayerHUD>().Init();
        PlayerInventory.Init();

        player.GetComponent<PlayerAnimator>().Init();
        player.GetComponent<PlayerInput>().Init();
        player.GetComponent<PlayerMovement>().Init();
        player.GetComponent<PlayerThreeDM>().Init();

        enemySpawning.Init();

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