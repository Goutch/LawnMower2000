using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject LocalOnlineMenuPanel = null;
    [SerializeField] private GameObject JoinRoomMenuPanel = null;
    [SerializeField] private GameObject OptionsMenuPanel = null;
    [SerializeField] private InputField RoomInputField = null;

    private GameManager gameManager;
    private Options options;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
    }

    public void OnPlayButtonClick()
    {
        MainMenuPanel.SetActive(false);
        LocalOnlineMenuPanel.SetActive(true);
    }

    public void OnLocalButtonClick()
    {
        options.GameMode = Options.GameModeType.OfflineVsAI;
        gameManager.LoadLocalGame();
    }

    public void OnOnlineButtonClick()
    {
        LocalOnlineMenuPanel.SetActive(false);
        JoinRoomMenuPanel.SetActive(true);
    }

    public void OnBackButtonLocalOnlineMenuPanelClick()
    {
        MainMenuPanel.SetActive(true);
        LocalOnlineMenuPanel.SetActive(false);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
    public void OnJoinButtonClick()
    {
        options.GameMode = Options.GameModeType.Online;
        gameManager.LoadOnlineGame(RoomInputField.text);
    }

    public void OnBackButtonJoinRoomMenu()
    {
        LocalOnlineMenuPanel.SetActive(true);
        JoinRoomMenuPanel.SetActive(false);
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
    
    

}