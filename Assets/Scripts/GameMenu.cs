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
    [SerializeField] private GameObject playerOneVictoryImage = null;
    [SerializeField] private GameObject playerTwoVictoryImage = null;
    [SerializeField] private Text playerOnePoints = null;
    [SerializeField] private Text playerTwoPoints = null;
    [SerializeField] private Image playerOneColor = null;
    [SerializeField] private Image playerTwoColor = null;
    [SerializeField] private GameObject warningText = null;
    [SerializeField] private Text timeText = null;
    [SerializeField] private EventSystem eventSystem = null;
    private GameManager gameManager;


    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        menuPanel.SetActive(false);
        endGamePanel.SetActive(false);
        warningText.SetActive(false);
    }

    private void OnEnable()
    {
        gameManager.OnGameFinish += GameManager_OnGameFinish;
        gameManager.OnWarning += GameManger_OnWarning;
    }

    private void OnDisable()
    {
        gameManager.OnWarning -= GameManger_OnWarning;
        gameManager.OnGameFinish -= GameManager_OnGameFinish;
    }

    private void GameManger_OnWarning()
    {
        StartCoroutine(setWarning());
    }

    private void GameManager_OnGameFinish()
    {
        endGamePanel.SetActive(true);
        playerOneVictoryImage.SetActive(false);
        playerTwoVictoryImage.SetActive(false);

        playerOneImage.color = gameManager.Player.Color;
        playerOneScore.text = gameManager.Player.Points.ToString();

        playerTwoImage.color = gameManager.AI.Color;
        playerTwoScore.text = gameManager.AI.Points.ToString();

        if (gameManager.Player.Points > gameManager.AI.Points)
        {
            playerOneVictoryImage.SetActive(true);
        }
        else if (gameManager.Player.Points < gameManager.AI.Points)
        {
            playerTwoVictoryImage.SetActive(true);
        }
        else
        {
            playerOneVictoryImage.SetActive(true);
            playerTwoVictoryImage.SetActive(true);
        }
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
                gameManager.PauseMusic();
                eventSystem.SetSelectedGameObject(backToMenuButton.gameObject);
            }
            else
            {
                Time.timeScale = 1;
                gameManager.PauseMusic();
            }
        }

        playerOneColor.color = gameManager.Player.Color;
        playerOnePoints.text = gameManager.Player.Points.ToString();

        playerTwoColor.color = gameManager.AI.Color;
        playerTwoPoints.text = gameManager.AI.Points.ToString();
    }

    public void OnBackToMenuButtonClick()
    {
        if (!gameManager.GameFinished)
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

    IEnumerator setWarning()
    {
        warningText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        warningText.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        warningText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        warningText.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        warningText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        warningText.SetActive(false);
    }
}