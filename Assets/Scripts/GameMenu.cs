using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel = null;
        [SerializeField] private Button backToMenuButton = null;
        [SerializeField] private GameObject pointsPanel = null;
        [SerializeField] private GameObject statsPrefab = null;
        [SerializeField] private Text timeText;
        private GameManager gameManager;
        private Options options;


        private void Start()
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            options = gameManager.GetComponent<Options>();
            backToMenuButton.onClick.AddListener(OnBackToMenuButtonClick);
            menuPanel.SetActive(false);

            gameManager.OnGameStart += GameManager_OnGameStart;
            gameManager.OnGameTimeChange += GameManager_OnGameTimeChange;
        }

        private void GameManager_OnGameTimeChange(float time)
        {
            timeText.text = "Time:"+((int)Mathf.Floor(time));
        }

        public void OnDisable()
        {
            gameManager.OnGameStart -= GameManager_OnGameStart;
            gameManager.OnGameTimeChange -= GameManager_OnGameTimeChange;
        }

        private void GameManager_OnGameStart()
        {
            int position = 0;
            foreach(LawnMower lawnMower in gameManager.LawnMowers)
            {
                StatsUpdate stats = Instantiate(statsPrefab, pointsPanel.transform).GetComponent<StatsUpdate>();
                stats.LawnMower = lawnMower;
                stats.Position = position++;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(options.InGameMenuButtonKey))
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