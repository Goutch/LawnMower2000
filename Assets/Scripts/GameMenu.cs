using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Text points1;
        [SerializeField] private Text points2;
        [SerializeField] private Image color1;
        [SerializeField] private Image color2;
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
            if (Input.GetKeyDown(options.InGameMenuButtonKey))
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
            }

            if (gameManager.IsGameActive())
            {
                LawnMower[] lawnMowers = gameManager.LawnMowers.ToArray();
                color1.color = options.LawnMower1Color;
                color2.color = options.LawnMower2Color;
                points1.text = ":" + lawnMowers[0].GetPoints();
                points2.text = ":" + lawnMowers[1].GetPoints();
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