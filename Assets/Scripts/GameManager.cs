using System;
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
    [SerializeField] private AudioClip menuMusic = null;
    [SerializeField] private AudioClip backgroundMusic = null;
    [SerializeField] private AudioClip secondBackgroundMusic = null;
    [SerializeField] private AudioClip birdOne = null;
    [SerializeField] private AudioClip birdTwo = null;
    [SerializeField] private AudioClip dog = null;

    #endregion

    #region Attribut
    public Map Map { get; private set; }
    public Options Options { get; private set; }

    public float RemaningTime { get { float timeElapsed = Time.time - startTime;  return timeElapsed > gameDuration ? 0 : gameDuration - timeElapsed; } }

    public LawnMower Player { get; private set; }
    public LawnMower AI { get; private set; }
    public bool GameStarted = false;
    public bool GameFinished = false;
    public bool menuMusicIsPlaying = false;
    public bool GameInProgress { get { return GameStarted && !GameFinished; } }
    public Sprite[] PlayerSpriteIdle = null;
    public Sprite[] PlayerGotPoints = null;
    #endregion

    #region private variable
    private float startTime;
    private float gameDuration;
    private bool warningPlayed;
    private bool mainSourceIsFading;
    private bool menuMusicIsFadingOut;
    private AudioSource mainAudioSource;
    private AudioSource menuAudioSource;
    private AudioSource effectsSource;
    private AudioSource environmentSource;
    private AudioClip[] environmentSounds;
    #endregion 

    #region Start Game

    void StartGame()
    {
        Map.Generate(Random.Range(0, int.MaxValue));
        gameDuration = (((Options.MapSize.x - 2) * (Options.MapSize.y - 2)) / 2) - 4;
        
        Player = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();
        AI = Instantiate(LawnMowerPrefab, Map.GetSpawnPoint(), quaternion.identity).GetComponent<LawnMower>();

        Player.Color = Options.LawnMower1Color;
        Player.GetComponent<LawnMower>().Idle = PlayerSpriteIdle;
        Player.GetComponent<LawnMower>().GotPoints = PlayerGotPoints;

        AI.Color = Options.LawnMower2Color;

        Player.gameObject.AddComponent<Player>();
        AI.gameObject.AddComponent<AI>();

        Instantiate(GameMenuPrefab);

        GameStarted = true;
        GameFinished = false;
        OnGameStart?.Invoke();

        startTime = Time.time;
        Player.engineAudioSource.volume = Options.lawnMowerVolume;
        Player.engineAudioSource.Play();
        AI.engineAudioSource.volume = Options.lawnMowerVolume;
        AI.engineAudioSource.Play();
        StartCoroutine(playEnvironmentalSounds());
    }

    void Start()
    {
        menuMusicIsFadingOut = false;
        environmentSounds = new AudioClip[]{birdOne,birdTwo,dog};
        Options = GetComponent<Options>();
        mainAudioSource = GetComponentInParent<AudioSource>();
        menuAudioSource = gameObject.AddComponent<AudioSource>();
        menuAudioSource.loop = true;
        effectsSource = gameObject.AddComponent<AudioSource>();
        environmentSource = gameObject.AddComponent<AudioSource>();
        environmentSource.volume = Options.environmentVolume;
        warningPlayed = false;
        mainSourceIsFading = false;
        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Additive);
        Options.volumeChanged += AdjustVolume;
    }

    private void OnDestroy()
    {
        Options.volumeChanged -= AdjustVolume;
    }

    public void LoadMenu()
    {
        SceneManager.UnloadSceneAsync("Game");
        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Additive);
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameAsynchronously());
        StartCoroutine(fadeOutMenuAudio());
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
        
        if (!GameStarted && !menuMusicIsPlaying && !effectsSource.isPlaying)
        {
            StartCoroutine(startMenuMusic());
        }
        
        if(RemaningTime == 0 && GameStarted && !GameFinished)
        {
            FinishGame();
        }
        else if(RemaningTime >20f)
        {
            mainAudioSource.pitch = 1f;
            mainAudioSource.volume = Options.musicVolume;
        }
        else if (RemaningTime <= 20f && !warningPlayed&& GameStarted && !GameFinished)
        {
            OnWarning?.Invoke();
            effectsSource.clip = warningSound;
            effectsSource.pitch = 0.8f;
            effectsSource.volume = Options.effectsVolume;
            StartCoroutine(warningCoroutine());
            warningPlayed = true;
        }
        else if(RemaningTime <=3f && !mainSourceIsFading && GameStarted &&!GameFinished)
        {
            StartCoroutine(fadeOutMainAudioEndGame());
        }
    }

    public void FinishGame()
    {
        Player.engineAudioSource.Stop();
        AI.engineAudioSource.Stop();
        effectsSource.clip = victorySound;
        mainAudioSource.Stop();
        StopAllCoroutines();
        OnGameFinish?.Invoke();
        GameFinished = true;
        GameStarted = false;
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
        Player.GetComponent<LawnMower>().Idle = PlayerSpriteIdle;
        Player.GetComponent<LawnMower>().GotPoints = PlayerGotPoints;

        AI.Color = Options.LawnMower2Color;

        Player.gameObject.AddComponent<Player>();
        AI.gameObject.AddComponent<AI>();

        GameStarted = true;
        GameFinished = false;
        OnGameStart?.Invoke();
        startTime = Time.time;
        effectsSource.Stop();
        warningPlayed = false;
        mainSourceIsFading = false;
        StartCoroutine(fadeOutMenuAudio());
        Player.engineAudioSource.volume = Options.lawnMowerVolume;
        Player.engineAudioSource.Play();
        AI.engineAudioSource.volume = Options.lawnMowerVolume;
        AI.engineAudioSource.Play();
        StartCoroutine(playEnvironmentalSounds());
    }

    IEnumerator warningCoroutine()
    {
        effectsSource.Play();
        yield return new WaitWhile (()=> effectsSource.isPlaying);
        mainAudioSource.pitch = 1.2f;
    }

    IEnumerator fadeOutMainAudioEndGame()
    {
        mainSourceIsFading = true;
        while (RemaningTime > 0f || mainAudioSource.volume >0)
        {
            mainAudioSource.volume -= Convert.ToSingle(0.2*mainAudioSource.volume);
            yield return new WaitForSeconds(1);
        }
        mainAudioSource.Stop();
    }

    IEnumerator fadeOutMenuAudio()
    {
        menuMusicIsFadingOut = true;
        StartCoroutine(fadeInMainAudioSource());
        while (menuAudioSource.volume > 0)
        {
            
            if (menuAudioSource.volume < 0.15)
            {
                menuAudioSource.volume = 0;
            }
            else
            {
                menuAudioSource.volume -= 0.15f;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        menuAudioSource.Stop();
        menuMusicIsPlaying = false;
        menuMusicIsFadingOut = false;
    }

    IEnumerator fadeInMainAudioSource()
    {
        int rnd = Random.Range(0, 2);

        if (rnd == 0)
        {
            mainAudioSource.clip = backgroundMusic;
        }
        else
        {
            mainAudioSource.clip = secondBackgroundMusic;
        }

        if (!mainAudioSource.isPlaying)
        {
            mainAudioSource.Play();
        }
        mainAudioSource.volume = 0;
        while (mainAudioSource.volume < Options.musicVolume)
        {
            mainAudioSource.volume += Convert.ToSingle(0.3 * Options.musicVolume);
            yield return new WaitForSeconds(1);
        }

        if (mainAudioSource.volume > Options.musicVolume)
        {
            mainAudioSource.volume = Options.musicVolume;
        }
    }

    IEnumerator startMenuMusic()
    {
        mainAudioSource.Stop();
        menuAudioSource.volume = 0f;
        menuAudioSource.clip = menuMusic;
        menuMusicIsPlaying = true;
        menuAudioSource.Play();
        while (menuAudioSource.volume < Options.musicVolume && !menuMusicIsFadingOut)
        {
            menuAudioSource.volume += 0.2f;
            yield return new WaitForSeconds(0.7f);
        }

        menuAudioSource.volume = Options.musicVolume;
    }

    IEnumerator playEnvironmentalSounds()
    {
        while (GameStarted && !GameFinished)
        {
            int rnd = Random.Range(0, environmentSounds.Length);
            yield return new WaitForSeconds(8);
            environmentSource.clip = environmentSounds[rnd];
            environmentSource.Play();
        }
    }


    public void PauseMusic()
    {
        if (mainAudioSource.isPlaying && GameStarted)
        {
            mainAudioSource.Pause();
            if (Player.engineAudioSource.isPlaying)
            {
                Player.engineAudioSource.Pause();
            }

            if (AI.engineAudioSource.isPlaying)
            {
                AI.engineAudioSource.Pause();
            }

            if (effectsSource.isPlaying)
            {
                effectsSource.Pause();
            }

            if (environmentSource.isPlaying)
            {
                environmentSource.Pause();
            }

        }
        else if(GameStarted)
        {
            mainAudioSource.Play();
            Player.engineAudioSource.Play();
            AI.engineAudioSource.Play();
            effectsSource.UnPause();
            environmentSource.UnPause();
        }
        
    }

    private void AdjustVolume()
    {
        mainAudioSource.volume = Options.musicVolume;
        effectsSource.volume = Options.effectsVolume;
        environmentSource.volume = Options.environmentVolume;
        menuAudioSource.volume = Options.musicVolume;
    }

}