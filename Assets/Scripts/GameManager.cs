using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Event
    public delegate void OnGameHandler();

    public event OnGameHandler OnGameStart;
    public event OnGameHandler OnGameFinish;
    #endregion

    #region SerializeField
    [SerializeField] private GameObject LoadingPanel = null;
    [SerializeField] private Text LoadingText = null;
    [SerializeField] private GameObject GameMenuPrefab = null;

    [SerializeField] private GameObject MapPrefab = null;
    [SerializeField] private GameObject LawnMowerPrefab = null;

    #endregion

    #region Attribut
    public Map Map { get; private set; }
    public Options Options { get; private set; }

    public float RemaningTime { get { float timeElapsed = Time.time - startTime;  return timeElapsed > gameDuration ? 0 : gameDuration - timeElapsed; } }

    public LawnMower Player { get; private set; }
    public LawnMower AI { get; private set; }
    public bool GameStarted = false;
    public bool GameFinished = false;
    public bool GameInProgress { get { return GameStarted && !GameFinished; } }
    #endregion

    #region private variable
    private float startTime;
    private float gameDuration;
    #endregion 

    #region Start Game

    void StartGame()
    {
        Map.Generate(Random.Range(0, int.MaxValue));
        //gameDuration = (((Options.MapSize.x - 2) * (Options.MapSize.y - 2)) / 2) - 4;
        gameDuration = 10;

        Player = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        AI = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

        Player.Color = Options.LawnMower1Color;
        AI.Color = Options.LawnMower2Color;

        Player.gameObject.AddComponent<Player>();
        AI.gameObject.AddComponent<AI>();

        Instantiate(GameMenuPrefab);

        GameStarted = true;
        GameFinished = false;
        OnGameStart?.Invoke();

        startTime = Time.time;
    }

    void Start()
    {
        Options = GetComponent<Options>();

        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Additive);
    }

    public void LoadMenu()
    {
        SceneManager.UnloadSceneAsync("Game");
        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Additive);
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameAsynchronously());
    }

    IEnumerator LoadGameAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);
        LoadingPanel.SetActive(true);

        SceneManager.UnloadSceneAsync("Menu");
        while (!operation.isDone)
        {
            LoadingText.text = operation.progress.ToString("P", CultureInfo.CreateSpecificCulture("en-US"));
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
        Map = Instantiate(MapPrefab, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        StartGame();
        LoadingText.text = "Loading completed";
        LoadingPanel.SetActive(false);
    }

    #endregion

    private void Update()
    {
        if(RemaningTime == 0 && GameStarted && !GameFinished)
        {
            FinishGame();
        }
    }

    public void FinishGame()
    {
        OnGameFinish?.Invoke();
        GameFinished = true;

        Destroy(Player.gameObject);
        Destroy(AI.gameObject);
    }

    public void RestartGame()
    {
        if (!GameFinished)
        {
            FinishGame();
        }

        Map.Generate(Random.Range(0,int.MaxValue));

        Player = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        AI = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

        Player.Color = Options.LawnMower1Color;
        AI.Color = Options.LawnMower2Color;

        Player.gameObject.AddComponent<Player>();
        AI.gameObject.AddComponent<AI>();

        GameStarted = true;
        GameFinished = false;
        OnGameStart?.Invoke();

        startTime = Time.time;
    }

   
}