using UnityEngine;
using System.IO;

public class Init : MonoBehaviour
{
    // hierarchy
    public AudioManager audioManager;
    public MusicController musicController;
    public Transform proceduralGeneration;
    public Transform misc;
    public Transform player;
    public Guns guns;
    public EnemySpawning enemySpawning;
    public Transform canvas;
    public DevConsole devConsole;
    public MenuHandler menuHandler;

    void Start()
    {
        Directory.CreateDirectory(SaveData.DIRECTORY_PATH);

        audioManager.Init();
        musicController.Init();
        Biome.Init();
        proceduralGeneration.GetComponent<ProceduralGeneration>().Init();

        misc.GetComponent<PauseHandler>().Init();
        menuHandler.Init();

        guns.Init();
        player.GetComponent<PlayerHUD>().Init();
        PlayerInventory.Init();

        player.GetComponent<PlayerAnimator>().Init();
        player.GetComponent<PlayerInput>().Init();
        player.GetComponent<PlayerMovement>().Init();
        player.GetComponent<PlayerThreeDM>().Init();

        enemySpawning.Init();

        devConsole.Init();

        PauseHandler.Pause();
        MenuHandler.CurrentMenu = 1;

        Destroy(gameObject);
    }
}