using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel = null;
        [SerializeField] private Button backToMenuButton = null;
        [SerializeField] private GameObject pointsPanel = null;
        [SerializeField] private GameObject statsPrefab = null;
        [SerializeField] private Text timeText = null;
        [SerializeField] private Text startGameText = null;
        private GameManager gameManager;
        private Options options;
        private bool menuIsOpened;

        private List<StatsUpdate> statsUpdates = new List<StatsUpdate>();

        private void Start()
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            options = gameManager.GetComponent<Options>();
            backToMenuButton.onClick.AddListener(OnBackToMenuButtonClick);
            menuPanel.SetActive(false);
            menuIsOpened = false;
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
            foreach(StatsUpdate stats in statsUpdates)
            {
                Destroy(stats.gameObject);
            }
            statsUpdates.Clear();

            int position = 0;
            foreach(LawnMower lawnMower in gameManager.LawnMowers)
            {
                StatsUpdate stats = Instantiate(statsPrefab, pointsPanel.transform).GetComponent<StatsUpdate>();
                stats.LawnMower = lawnMower;
                stats.Position = position++;
                statsUpdates.Add(stats);
            }
        }

        private void Update()
        {
            if(gameManager.Player != null && !gameManager.Player.Ready)
            {
                startGameText.gameObject.SetActive(true);
            }
            else
            {
                startGameText.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(options.InGameMenuButtonKey)|| Input.GetButton("Menu"))
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
                gameManager.SetInGameMenuState(menuPanel.activeSelf);
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
