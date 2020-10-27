using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mapPrefab;
    private bool gameSceneActive=false;
    
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadMenu();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync("Menu");
            gameSceneActive = true;
            StartGame();
        }

        if (scene.name == "Menu")
        {
            SceneManager.SetActiveScene(scene);
            if (gameSceneActive)
            {
                SceneManager.UnloadSceneAsync("Game");
            }
        }
    }

    private void StartGame()
    {
        //generate map
        Instantiate(mapPrefab,Vector3.zero, Quaternion.identity);
        //initialise lawnmowers and place them
        //link players with their lawnmower
    }

    private void EndGame()
    {
        //show winner 
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Additive);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Scenes/Menu",LoadSceneMode.Additive);
    }
}
