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
    [SerializeField] private Text playerOneScore = null;
    [SerializeField] private Text playerTwoScore = null;
    [SerializeField] private Image playerOneImage = null;
    [SerializeField] private Image playerTwoImage = null;

    [SerializeField] private Text playerOnePoints = null;
    [SerializeField] private Text playerTwoPoints = null;
    [SerializeField] private Image playerOneColor = null;
    [SerializeField] private Image playerTwoColor = null;

    [SerializeField] private Text timeText = null;

    private GameManager gameManager;
    private EventSystem eventSystem;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        menuPanel.SetActive(false);
        endGamePanel.SetActive(false);
        eventSystem = menuPanel.GetComponentInChildren<EventSystem>();
    }

    private void OnEnable()
    {
        gameManager.OnGameFinish += GameManager_OnGameFinish;
    }

    private void OnDisable()
    {
        gameManager.OnGameFinish -= GameManager_OnGameFinish;
    }

    private void GameManager_OnGameFinish()
    {
        endGamePanel.SetActive(true);

        playerOneImage.color = gameManager.Player.Color;
        playerOneScore.text = gameManager.Player.Points.ToString();

        playerTwoImage.color = gameManager.AI.Color;
        playerTwoScore.text = gameManager.AI.Points.ToString();
    }

    private void Update()
    {
        timeText.text = "Time: " + gameManager.RemaningTime.ToString("F");

        if (Input.GetKeyDown(gameManager.Options.Controls.gameMenu) || Input.GetButtonDown("Menu"))
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            if (menuPanel.activeSelf)
            {
                Time.timeScale = 0;

                EventSystem.current = eventSystem;
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(backToMenuButton.gameObject);
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        playerOneColor.color = gameManager.Player.Color;
        playerOnePoints.text = gameManager.Player.Points.ToString();

        playerTwoColor.color = gameManager.AI.Color;
        playerTwoPoints.text = gameManager.AI.Points.ToString();
    }

    public void OnBackToMenuButtonClick()
    {
        if(!gameManager.GameFinished)
        {
            gameManager.FinishGame();
        }
        
        menuPanel.SetActive(false);
        endGamePanel.SetActive(false);
        gameManager.LoadMenu();

        Time.timeScale = 1;
    }

    public void OnPlayAgainButtonClicked()
    {
        gameManager.RestartGame();
        endGamePanel.SetActive(false);
    }
}