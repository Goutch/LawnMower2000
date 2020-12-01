using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject OptionsMenuPanel = null;

    private GameManager gameManager;
    private Options options;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
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
}