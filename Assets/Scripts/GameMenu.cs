using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel = null;
    [SerializeField] private Button backToMenuButton = null;
    [SerializeField] private GameObject endGamePanel = null;
    [SerializeField] private GameObject pointsPanel = null;
    [SerializeField] private GameObject statsPrefab = null;
    [SerializeField] private Text timeText = null;
    [SerializeField] private Text startGameText = null;
    [SerializeField] private Text playerOneScore = null;
    [SerializeField] private Text playerTwoScore = null;
    [SerializeField] private Image playerOneImage = null;
    [SerializeField] private Image playerTwoImage = null;
    private GameManager gameManager;
    private Options options;
    private bool menuIsOpened;
    private EventSystem eventSystem;
    private List<StatsUpdate> statsUpdates = new List<StatsUpdate>();

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        menuPanel.SetActive(false);
        endGamePanel.SetActive(false);
        menuIsOpened = false;
        gameManager.OnGameStart += GameManager_OnGameStart;
        gameManager.OnGameTimeChange += GameManager_OnGameTimeChange;
        gameManager.OnGameFinish += OnGameFinished;
        eventSystem = menuPanel.GetComponentInChildren<EventSystem>();
    }

    private void GameManager_OnGameTimeChange(float time)
    {
        timeText.text = "Time:" + ((int) Mathf.Floor(time));
    }

    public void OnDisable()
    {
        gameManager.OnGameStart -= GameManager_OnGameStart;
        gameManager.OnGameTimeChange -= GameManager_OnGameTimeChange;
        gameManager.OnGameFinish -= OnGameFinished;
    }

    private void GameManager_OnGameStart()
    {
        foreach (StatsUpdate stats in statsUpdates)
        {
            Destroy(stats.gameObject);
        }

        statsUpdates.Clear();

        int position = 0;
        foreach (LawnMower lawnMower in gameManager.LawnMowers)
        {
            StatsUpdate stats = Instantiate(statsPrefab, pointsPanel.transform).GetComponent<StatsUpdate>();
            stats.LawnMower = lawnMower;
            stats.Position = position++;
            statsUpdates.Add(stats);
        }
    }

    private void Update()
    {
        if (gameManager.Player != null && !gameManager.Player.Ready)
        {
            startGameText.gameObject.SetActive(true);
        }
        else
        {
            startGameText.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(options.InGameMenuButtonKey) || Input.GetButtonDown("Menu"))
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            if (menuPanel.activeSelf)
            {
                if (!gameManager.IsGameOnline())
                {
                    Time.timeScale = 0;
                }

                EventSystem.current = eventSystem;
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(backToMenuButton.gameObject);
            }
            else
            {
                Time.timeScale = 1;
            }

            gameManager.SetInGameMenuState(menuPanel.activeSelf);
        }
    }

    public void OnBackToMenuButtonClick()
    {
        Time.timeScale = 1;
        gameManager.FinishGame();
        gameManager.LoadMenu();
        gameManager.SetInGameMenuState(false);
    }

    public void OnGameFinished()
    {
        if (!endGamePanel.activeSelf)
        {
            endGamePanel.SetActive(true);
             
            for (int i=0;i< gameManager.LawnMowers.Count; i++)
            {
                if (i == 0)
                {
                    playerOneImage.color = gameManager.LawnMowers[i].Color;
                    playerOneScore.text = gameManager.LawnMowers[i].Points.ToString();
                }
                else
                {
                    playerTwoImage.color = gameManager.LawnMowers[i].Color;
                    playerTwoScore.text = gameManager.LawnMowers[i].Points.ToString();
                }
            }
        }
    }

    public void OnPlayAgainButtonClicked()
    {
        endGamePanel.SetActive(false);
        gameManager.FinishGame();
    }
}