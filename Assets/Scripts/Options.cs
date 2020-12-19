using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Options : MonoBehaviour
{
    private const string CONTROLS_PATH = "./Controls.json";

    public enum GameModeType
    {
        OfflineVsAI,
        Online,
    }

    public KeyBinding Controls { private set; get; }
    public GameModeType GameMode { set; get; } = GameModeType.OfflineVsAI;
    public Color LawnMower1Color  = Color.blue;
    public Color LawnMower2Color  = Color.red;
    public Vector2Int MapSize { private set; get; } = new Vector2Int(16, 16);


    private Dictionary<string, KeyCode> temporaryBindings = new Dictionary<string, KeyCode>();

    private void Start()
    {
        ReadKeyBindings();
    }

    public void ApplyKeyBindings()
    {
        Controls.right = temporaryBindings["R"];
        Controls.left = temporaryBindings["L"];
        Controls.startGame = temporaryBindings["S"];
        Controls.gameMenu = temporaryBindings["M"];
        Controls.foward = temporaryBindings["C"];
        WriteKeyBindings();
    }

    public void SetKeyBinding(string key, KeyCode keyCode)
    {
        temporaryBindings[key] = keyCode;
    }

    private void WriteKeyBindings()
    {
        string json = JsonUtility.ToJson(Controls);
        StreamWriter writer = new StreamWriter(CONTROLS_PATH, false);
        writer.Write(json);
        writer.Close();
    }

    public void ReadKeyBindings()
    {
        if (File.Exists(CONTROLS_PATH))
        {
            StreamReader reader = new StreamReader(CONTROLS_PATH);
            Controls = JsonUtility.FromJson<KeyBinding>(reader.ReadToEnd());
            reader.Close();
        }
        else
        {
            Controls = new KeyBinding();
        }

        temporaryBindings["R"] = Controls.right;
        temporaryBindings["L"] = Controls.left;
        temporaryBindings["S"] = Controls.startGame;
        temporaryBindings["M"] = Controls.gameMenu;
        temporaryBindings["C"] = Controls.foward;
    }

    public void SetDefaultKeyBindings()
    {
        Controls = new KeyBinding();
        temporaryBindings["R"] = Controls.right;
        temporaryBindings["L"] = Controls.left;
        temporaryBindings["S"] = Controls.startGame;
        temporaryBindings["M"] = Controls.gameMenu;
        temporaryBindings["C"] = Controls.foward;
    }

    public KeyCode getKeyBinding(string key)
    {
        return temporaryBindings[key];
    }
}