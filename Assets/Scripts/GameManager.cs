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

    private void StartNetworkGame()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        mapPrefab.AddComponent<PhotonView>();
        pool.ResourceCache.Add("GameMap", mapPrefab);

        Map map = PhotonNetwork.Instantiate("GameMap", Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init();
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

    private void OnJoinedRoom(object sender)
    {
        NetworkManager networkManager = (NetworkManager)sender;
        networkManager.OnJoinedRoomEvent -= OnJoinedRoom;

        StartCoroutine(LoadOnlineGameAsynchronously("Scenes/Game"));
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
            if (gameSceneActive)
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

        StartNetworkGame();
    }
}