using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject OptionsMenuPanel = null;
    [SerializeField] private GameObject AvatarSelectionMenu = null;
    [SerializeField] private AvatarSelectionMenu AvatarSelectionScript = null;
    [SerializeField] private Animator mainMenuAnimator = null;

    private GameManager gameManager;
    private Options options;

    public void Start()
    {
        AvatarSelectionScript.OnStartEvent += MainMenu_OnStartEvent;

        if (mainMenuAnimator != null)
        {
            mainMenuAnimator.SetBool("Opened",true);
        }
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
        mainMenuAnimator.SetBool("Opened",false);
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
        mainMenuAnimator.SetBool("Opened",false);
        OptionsMenuPanel.SetActive(true);
    }

    public void OnBackButtonOptionsClick()
    {
        OptionsMenuPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        mainMenuAnimator.SetBool("Opened",true);
    }

    public void OnBackButtonAvatarClick()
    {
        AvatarSelectionMenu.SetActive(false);
        MainMenuPanel.SetActive(true);
        mainMenuAnimator.SetBool("Opened",true);
    }

    private void OnStartGameButtonClick()
    {
        mainMenuAnimator.SetBool("Opened",false);
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        gameManager.LoadGame();
    }
}