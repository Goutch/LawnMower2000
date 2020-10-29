using UnityEngine;

public class Options : MonoBehaviour
{
    public enum GameModeType
    {
        OfflineVsAI,
        Online,
    }

    public GameModeType GameMode { set; get; }  = GameModeType.OfflineVsAI;
    public Color LawnMower1Color { private set; get; } = Color.blue;
    public Color LawnMower2Color { private set; get; } = Color.red;
    public KeyCode InGameMenuButtonKey { private set; get; } = KeyCode.Escape;
    public KeyCode ContinueKey { private set; get; }  = KeyCode.W;
    public KeyCode TurnLeftKey { private set; get; } = KeyCode.A;
    public KeyCode TurnRightKey { private set; get; } = KeyCode.D;
    public Vector2Int MapSize { private set; get; } = new Vector2Int(16, 16);
}