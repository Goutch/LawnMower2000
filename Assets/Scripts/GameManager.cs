using System.Net.NetworkInformation;
using DefaultNamespace;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Options;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    [SerializeField] private GameObject lawnMowerPrefab;
    [SerializeField] private GameObject gameMenuPrefab;

    private Options options;
    private bool gameSceneActive = false;
    private LawnMower[] lawnMowers;
    private Map map;

    void Start()
    {
        options = GetComponent<Options>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadMenu();
    }

    private void StartNetworkGame()
    {

    }

    private void StartGame()
    {
        //Map
        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init();
        //Lawnmowers
        lawnMowers = new LawnMower[2];
        lawnMowers[0] = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        lawnMowers[1] = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
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
        
        //Menu
        Instantiate(gameMenuPrefab);

    }

    private void OnJoinedRoom(object sender)
    {
        NetworkManager networkManager = (NetworkManager)sender;
        networkManager.OnJoinedRoomEvent -= OnJoinedRoom;

        StartCoroutine(LoadOnlineGameAsynchronously("Scenes/Game"));
    }

    public LawnMower[] GetLawnmowers()
    {
        return lawnMowers;
    }
    
    public bool IsGameActive()
    {
        return gameSceneActive;
    }

    public void LoadOnlineGame(string roomName)
    {
        options.GameMode = GameModeType.Online;

        NetworkManager networkManager = new GameObject().AddComponent<NetworkManager>();
        networkManager.name = "NetworkManager";
        Scene scene = SceneManager.GetSceneByName("Main");
        SceneManager.MoveGameObjectToScene(networkManager.gameObject, scene);

        networkManager.OnJoinedRoomEvent += OnJoinedRoom;
        networkManager.Connect(roomName);
    }

    public void LoadLocalGame()
    {
        options.GameMode = GameModeType.OfflineVsAI;
        StartCoroutine(LoadLocalGameAsynchronously("Scenes/Game"));
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

            if(options.GameMode == GameModeType.OfflineVsAI)
            {
                StartGame();
            }
        }

        if (scene.name == "Menu")
        {
            SceneManager.SetActiveScene(scene);
            if (IsGameActive())
            {
                SceneManager.UnloadSceneAsync("Game");
            }
        }
    }

    IEnumerator LoadLocalGameAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadOnlineGameAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (operation.isDone)
        {
            StartNetworkGame();
        }
        
    }

    public Map GetMap()
    {
        return map;
    }
}