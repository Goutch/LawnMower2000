using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    [SerializeField] private GameObject lawnMowerPrefab;
    [SerializeField] private GameObject gameMenuPrefab;
    [SerializeField] private float gameDuration;
    [SerializeField] private GameObject titleScreenPrefab;
    private Options options;
    private bool gameSceneActive = false;
    private Map map;
    private bool inGameMenuIsOpened = false;
    private bool titleScreenShowed = false;

    public delegate void OnGameStartHandler();

    public delegate void OnGameFinishHandler();

    public event OnGameStartHandler OnGameStart;
    public event OnGameFinishHandler OnGameFinish;

    public delegate void OnGameTimeChangeHandler(float time);

    public event OnGameTimeChangeHandler OnGameTimeChange;
    public bool GameStarted { set; get; } = false;
    public bool GameFinished { set; get; } = false;
    public List<LawnMower> LawnMowers { private set; get; } = new List<LawnMower>();

    private float gameTime = 0f;

    public LawnMower Player = null;

    void Update()
    {
        if (!GameStarted && !GameFinished)
        {
            LawnMowers = GameObject.FindGameObjectsWithTag("LawnMower").Select(x => x.GetComponent<LawnMower>()).ToList();

            if (LawnMowers.Count != 0 && LawnMowers.All(l => l.Ready))
            {
                GameStarted = true;

                gameTime = gameDuration;
                OnGameStart?.Invoke();
            }
        }

        if (GameStarted)
        {
            gameTime -= Time.deltaTime;
            if (gameTime < 0)
            {
                gameTime = 0;
                GameStarted = false;
                GameFinished = true;
            }

            OnGameTimeChange?.Invoke(gameTime);
        }

        if (GameFinished)
        {
            OnGameFinish?.Invoke();
        }
    }

    public void FinishGame()
    {
        gameTime = 0;
        if (options.GameMode == Options.GameModeType.Online)
        {
            PhotonNetwork.Destroy(Player.GetComponentInParent<PhotonView>());
            LawnMowers.Clear();
            map.Init(0);

            LawnMower lawnMower1 = PhotonNetwork.Instantiate("NetworkLawnMower", map.GetSpawnPoint(), quaternion.identity).GetComponentInChildren<LawnMower>();
            Player = lawnMower1;

            lawnMower1.OrientationLawnMower = LawnMower.Orientation.up;
            lawnMower1.gameObject.AddComponent<Player>();

            GameFinished = false;
        }

        if (options.GameMode == Options.GameModeType.OfflineVsAI)
        {
            foreach (LawnMower mower in LawnMowers)
            {
                Destroy(mower.gameObject);
            }

            LawnMowers.Clear();

            map.Init(0);

            LawnMower lawnMower1 = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
            LawnMower lawnMower2 = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

            lawnMower1.Color = options.LawnMower1Color;
            lawnMower2.Color = options.LawnMower2Color;
            lawnMower1.OrientationLawnMower = LawnMower.Orientation.up;
            lawnMower2.OrientationLawnMower = LawnMower.Orientation.up;

            lawnMower1.gameObject.AddComponent<Player>();
            Player = lawnMower1;
            lawnMower2.gameObject.AddComponent<AI>();

            GameFinished = false;
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
        options.GameMode = Options.GameModeType.Online;

        //Map
        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init(0);

        LawnMower lawnMower1 = PhotonNetwork.Instantiate("NetworkLawnMower", map.GetSpawnPoint(), quaternion.identity).GetComponentInChildren<LawnMower>();
        Player = lawnMower1;

        lawnMower1.OrientationLawnMower = LawnMower.Orientation.up;
        lawnMower1.gameObject.AddComponent<Player>();

        Instantiate(gameMenuPrefab);
    }

    private void StartGame()
    {
        options.GameMode = Options.GameModeType.OfflineVsAI;

        //Map
        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        map.Init(0);

        LawnMower lawnMower1 = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        LawnMower lawnMower2 = Instantiate(lawnMowerPrefab, map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

        lawnMower1.Color = options.LawnMower1Color;
        lawnMower2.Color = options.LawnMower2Color;
        lawnMower1.OrientationLawnMower = LawnMower.Orientation.up;
        lawnMower2.OrientationLawnMower = LawnMower.Orientation.up;

        lawnMower1.gameObject.AddComponent<Player>();
        Player = lawnMower1;
        lawnMower2.gameObject.AddComponent<AI>();

        Instantiate(gameMenuPrefab);
    }

    private void OnJoinedRoom(object sender)
    {
        NetworkManager networkManager = (NetworkManager) sender;
        networkManager.OnJoinedRoomEvent -= OnJoinedRoom;
        SceneManager.UnloadSceneAsync("Scenes/Loading");
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
            GameStarted = false;
            GameFinished = false;
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync("Menu");
            gameSceneActive = true;
        }

        if (scene.name == "Menu")
        {
            SceneManager.SetActiveScene(scene);
            if (!titleScreenShowed)
            {
                titleScreenShowed = true;
                Instantiate(titleScreenPrefab, Vector3.zero, Quaternion.identity);
            }

            if (IsGameActive())
            {
                SceneManager.UnloadSceneAsync("Game");
            }
        }
    }

    IEnumerator LoadLocalGameAsynchronously()
    {
        SceneManager.LoadScene("Scenes/Loading", LoadSceneMode.Additive);
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        StartGame(); 
        SceneManager.UnloadSceneAsync("Scenes/Loading");
     
    }

    IEnumerator LoadOnlineGameAsynchronously(string roomName)
    {
        SceneManager.LoadScene("Scenes/Loading", LoadSceneMode.Additive);
        
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        NetworkManager networkManager = new GameObject().AddComponent<NetworkManager>();
        networkManager.name = "NetworkManager";
        networkManager.tag = "NetworkManager";
        Scene scene = SceneManager.GetSceneByName("Main");
        SceneManager.MoveGameObjectToScene(networkManager.gameObject, scene);

        networkManager.OnJoinedRoomEvent += OnJoinedRoom;
        networkManager.Connect(roomName);

    }

    public Map GetMap()
    {
        return map;
    }

    public void SetInGameMenuState(bool enabled)
    {
        inGameMenuIsOpened = enabled;
        Player.GetComponent<Player>().TogglePlayerInputs(enabled);
    }

    public bool IsMenuOpen()
    {
        return inGameMenuIsOpened;
    }

    public bool IsGameOnline()
    {
        return options.GameMode == Options.GameModeType.Online;
    }
}