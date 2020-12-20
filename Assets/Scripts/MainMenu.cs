using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject OptionsMenuPanel = null;
    [SerializeField] private GameObject AvatarSelectionMenu = null;
    [SerializeField] private AvatarSelectionMenu AvatarSelectionScript = null;

    private GameManager gameManager;
    private Options options;

    public void Start()
    {
        AvatarSelectionScript.OnStartEvent += MainMenu_OnStartEvent;

    }

    public void Destroy()
    {
        AvatarSelectionScript.OnStartEvent -= MainMenu_OnStartEvent;
    }

    private void MainMenu_OnStartEvent()
    {
        OnStartGameButtonClick();
    }

    public void OnPlayButtonClick()
    {
        MainMenuPanel.SetActive(false);

        AvatarSelectionMenu.SetActive(true);
    }

    public void OnLocalButtonClick()
    {
        MainMenuPanel.SetActive(false);
        gameManager.LoadGame();
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnOptionsButtonClick()
    {
        MainMenuPanel.SetActive(false);
        OptionsMenuPanel.SetActive(true);
    }

    public void OnBackButtonOptionsClick()
    {
        OptionsMenuPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void OnBackButtonAvatarClick()
    {
        AvatarSelectionMenu.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    private void OnStartGameButtonClick()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        gameManager.LoadGame();
    }
}