using UnityEngine;

public class Options : MonoBehaviour
{
    /*
    * Contains options ( the controls, the game mode ex: online/AI) 
    */
    public KeyCode InGameMenuButton { private set; get; } = KeyCode.Escape;
    public Vector2Int MapSize { private set; get; } = new Vector2Int(16, 16);
}