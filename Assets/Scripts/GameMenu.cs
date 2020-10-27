using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button backToMenuButton;
        private GameManager gameManager;
        private Options options;

        private void Start()
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            options = gameManager.GetComponent<Options>();
            backToMenuButton.onClick.AddListener(OnBackToMenuButtonClick);
            menuPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(options.InGameMenuButton))
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
            }
        }

        private void OnDestroy()
        {
            backToMenuButton.onClick.RemoveListener(OnBackToMenuButtonClick);
        }

        private void OnBackToMenuButtonClick()
        {
            gameManager.LoadMenu();
        }
    }
}