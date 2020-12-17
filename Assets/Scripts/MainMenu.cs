using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject OptionsMenuPanel = null;
    [SerializeField] private GameObject AvatarSelectionMenu = null;
    [SerializeField] private AvatarSelectionMenu AvatarSelectionSCript = null;

    private GameManager gameManager;
    private Options options;

    public void Start()
    {
        AvatarSelectionSCript.OnStartEvent += MainMenu_OnStartEvent;
    }

    public void Destroy()
    {
        AvatarSelectionSCript.OnStartEvent -= MainMenu_OnStartEvent;
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

    private void OnStartGameButtonClick()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        gameManager.LoadGame();
    }
}