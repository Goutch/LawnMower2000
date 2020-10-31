
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    [SerializeField] private GameObject lawnMowerPrefab;
    [SerializeField] private GameObject gameMenuPrefab;

    private Options options;
    private bool gameSceneActive = false;
    private Map map;


    public delegate void OnGameStartHandler();
    public event OnGameStartHandler OnGameStart;

    public bool GameStarted { private set; get; } = false;
    public List<LawnMower> LawnMowers { private set; get; } = new List<LawnMower>();

    void Update()
    {
        if (!GameStarted)
        {
            LawnMowers = GameObject.FindGameObjectsWithTag("LawnMower").Select(x => x.GetComponent<LawnMower>()).ToList();

            if (LawnMowers.Count != 0 && LawnMowers.All(l => l.Ready))
            {
                GameStarted = true;
                OnGameStart?.Invoke();
            }
        }
    }

    void Start()
    {
        options = GetComponent<Options>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadMenu();
    }

    private void StartNetworkGame()
    {
        //Map
        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init(0);

        LawnMower lawnMower1 = PhotonNetwork.Instantiate("NetworkLawnMower",map.GetSpawnPoint(), quaternion.identity).GetComponentInChildren<LawnMower>();

        lawnMower1.GetComponentInChildren<SpriteRenderer>().color = options.LawnMower1Color;
        lawnMower1.SetOrientation(LawnMower.Orientation.up);

        Instantiate(gameMenuPrefab);
    }

    private void StartGame()
    {
        //Map
        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init(0);

        LawnMower lawnMower1 = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        LawnMower lawnMower2 = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

        lawnMower1.GetComponentInChildren<SpriteRenderer>().color = options.LawnMower1Color;
        lawnMower2.GetComponentInChildren<SpriteRenderer>().color = options.LawnMower2Color;
        lawnMower1.SetOrientation(LawnMower.Orientation.up);
        lawnMower2.SetOrientation(LawnMower.Orientation.up);

        lawnMower1.gameObject.AddComponent<Player>();
        lawnMower2.gameObject.AddComponent<AI>();

        Instantiate(gameMenuPrefab);
    }

    private void OnJoinedRoom(object sender)
    {
        NetworkManager networkManager = (NetworkManager)sender;
        networkManager.OnJoinedRoomEvent -= OnJoinedRoom;

        StartNetworkGame();
    }
    
    public bool IsGameActive()
    {
        return gameSceneActive;
    }

    public void LoadOnlineGame(string roomName)
    {
        StartCoroutine(LoadOnlineGameAsynchronously(roomName));
    }

    public void LoadLocalGame()
    {
        StartCoroutine(LoadLocalGameAsynchronously());
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

    IEnumerator LoadLocalGameAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }
        StartGame();
    }

    IEnumerator LoadOnlineGameAsynchronously(string roomName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        NetworkManager networkManager = new GameObject().AddComponent<NetworkManager>();
        networkManager.name = "NetworkManager";
        Scene scene = SceneManager.GetSceneByName("Main");
        SceneManager.MoveGameObjectToScene(networkManager.gameObject, scene);

        networkManager.OnJoinedRoomEvent += OnJoinedRoom;
        networkManager.Connect(roomName);
    }

    public Map GetMap()
    {
        return map;
    }
}