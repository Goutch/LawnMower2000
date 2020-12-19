using System.Collections;
using System.Globalization;
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
    public event OnGameHandler OnWarning;
    #endregion

    #region SerializeField
    [SerializeField] private GameObject LoadingPanel = null;
    [SerializeField] private Text LoadingText = null;
    [SerializeField] private GameObject GameMenuPrefab = null;
    [SerializeField] private GameObject MapPrefab = null;
    [SerializeField] private GameObject LawnMowerPrefab = null;
    [SerializeField] private AudioClip warningSound = null;
    [SerializeField] private AudioClip victorySound = null;

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
    public Sprite PlayerSprite = null;
    #endregion

    #region private variable
    private float startTime;
    private float gameDuration;
    private bool warningPlayed;
    private bool mainSourceIsFading;
    private AudioSource mainAudioSource;
    private AudioSource effectsSource;
    #endregion 

    #region Start Game

    void StartGame()
    {
        Map.Generate(Random.Range(0, int.MaxValue));
        //gameDuration = (((Options.MapSize.x - 2) * (Options.MapSize.y - 2)) / 2) - 4;
        gameDuration = 30;
        
        Player = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        AI = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

        Player.Color = Options.LawnMower1Color;
        if (PlayerSprite != null)
        {
            Player.GetComponentInChildren<SpriteRenderer>().sprite = PlayerSprite;
        }

        AI.Color = Options.LawnMower2Color;

        Player.gameObject.AddComponent<Player>();
        AI.gameObject.AddComponent<AI>();

        Instantiate(GameMenuPrefab);

        GameStarted = true;
        GameFinished = false;
        OnGameStart?.Invoke();

        startTime = Time.time;
        mainAudioSource.Play();
    }

    void Start()
    {
        Options = GetComponent<Options>();
        mainAudioSource = GetComponentInParent<AudioSource>();
        effectsSource = gameObject.AddComponent<AudioSource>();
        warningPlayed = false;
        mainSourceIsFading = false;
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
        else if(RemaningTime >20f)
        {
            mainAudioSource.pitch = 1f;
            mainAudioSource.volume = 1f;
        }
        else if (RemaningTime <= 20f && !warningPlayed&& GameStarted && !GameFinished)
        {
            OnWarning?.Invoke();
            effectsSource.clip = warningSound;
            effectsSource.pitch = 0.8f;
            effectsSource.volume = 0.4f;
            StartCoroutine(warningCoroutine());
            warningPlayed = true;
        }
        else if(RemaningTime <=3f && !mainSourceIsFading && GameStarted &&!GameFinished)
        {
            StartCoroutine(fadeOutMainAudio());
        }
    }

    public void FinishGame()
    {
        effectsSource.clip = victorySound;
        mainAudioSource.Stop();
        StopAllCoroutines();
        OnGameFinish?.Invoke();
        GameFinished = true;
        if (RemaningTime <= 0)
        {
            effectsSource.Play();
        }

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
        if (PlayerSprite != null)
        {
            Player.GetComponentInChildren<SpriteRenderer>().sprite = PlayerSprite;
        }
        AI.Color = Options.LawnMower2Color;

        Player.gameObject.AddComponent<Player>();
        AI.gameObject.AddComponent<AI>();

        GameStarted = true;
        GameFinished = false;
        OnGameStart?.Invoke();
        startTime = Time.time;
        effectsSource.Stop();
        mainAudioSource.Play();
        warningPlayed = false;
        mainSourceIsFading = false;
    }

    IEnumerator warningCoroutine()
    {
        effectsSource.Play();
        yield return new WaitWhile (()=> effectsSource.isPlaying);
        mainAudioSource.pitch = 1.2f;
    }

    IEnumerator fadeOutMainAudio()
    {
        mainSourceIsFading = true;
        while (RemaningTime > 0f || mainAudioSource.volume >0)
        {
            mainAudioSource.volume -= 0.2f;
            yield return new WaitForSeconds(1);
        }
    }

    public void PauseMusic()
    {
        if (mainAudioSource.isPlaying && GameStarted)
        {
            mainAudioSource.Pause();
        }
        else if(GameStarted)
        {
            mainAudioSource.Play();
        }
    }

}