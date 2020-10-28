using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    [SerializeField] private GameObject LawnMowerPrefab;
    private Options options;
    private bool gameSceneActive = false;
    private LawnMower[] lawnMowers;
    void Start()
    {
        options = GetComponent<Options>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadMenu();
    }



    private void StartGame()
    {
        //Map
        Map map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init();
        //Lawnmowers
        lawnMowers=new LawnMower[2];
        lawnMowers[0] = Instantiate(LawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        lawnMowers[1] = Instantiate(LawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        lawnMowers[0].SetMap(map);
        lawnMowers[1].SetMap(map);
        lawnMowers[0].GetComponentInChildren<SpriteRenderer>().color = options.LawnMower1Color;
        lawnMowers[1].GetComponentInChildren<SpriteRenderer>().color = options.LawnMower2Color;

        //Players
        lawnMowers[0].gameObject.AddComponent<Player>();
        switch (options.GameMode)
        {
            case Options.GameModeType.Online:
                lawnMowers[1].gameObject.AddComponent<RemotePlayer>();
                break;
            case Options.GameModeType.OfflineVsAI:
                lawnMowers[1].gameObject.AddComponent<AI>();
                break;
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Additive);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Additive);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync("Menu");
            gameSceneActive = true;
            StartGame();
        }

        if (scene.name == "Menu")
        {
            SceneManager.SetActiveScene(scene);
            if (gameSceneActive)
            {
                SceneManager.UnloadSceneAsync("Game");
            }
        }
    }
}