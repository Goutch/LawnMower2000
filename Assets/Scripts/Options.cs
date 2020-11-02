using System.Collections;
using UnityEngine;
using System.IO;

public class Options : MonoBehaviour
{
    public enum GameModeType
    {
        OfflineVsAI,
        Online,
    }

    public const int AMOUNT_OF_KEY_BINDINGS = 4; //0 = start
    
    public GameModeType GameMode { set; get; }  = GameModeType.OfflineVsAI;
    public Color LawnMower1Color { private set; get; } = Color.blue;
    public Color LawnMower2Color { private set; get; } = Color.red;
    public KeyCode InGameMenuButtonKey { private set; get; } = KeyCode.Escape;
    public KeyCode ContinueKey { private set; get; }  = KeyCode.W;
    public KeyCode TurnLeftKey { private set; get; } = KeyCode.A;
    public KeyCode TurnRightKey { private set; get; } = KeyCode.D;
    public KeyCode ReadyKey { private set; get; } = KeyCode.Return;
    public Vector2Int MapSize { private set; get; } = new Vector2Int(16, 16);

    
    private Hashtable bindings;

    public void ReadKeyBindings()
    {
        string path = "Assets/Resources/Controls.txt";
        StreamReader reader = new StreamReader(path);
        string line; 
        bindings = new Hashtable();
        
        while ((line = reader.ReadLine()) != null)
        {
            string[] txtLine = line.Split('=');
            bindings.Add(txtLine[0],txtLine[1]);
        }
        
        Debug.Log(reader.ReadToEnd());
        reader.Close();

        if (bindings.Count < AMOUNT_OF_KEY_BINDINGS)
        {
            SetDefaultKeyBindings();
        }
    }

    public void ApplyKeyBindings()
    {
        TurnRightKey = (KeyCode)bindings["R"];
        TurnLeftKey = (KeyCode) bindings["L"];
        ReadyKey = (KeyCode) bindings["S"];
        InGameMenuButtonKey = (KeyCode) bindings["M"];
        ContinueKey = (KeyCode) bindings["C"];


    }

    private void SetKeyBinding(string key, KeyCode keyCode)
    {
        bindings[key] = keyCode;
        string path = "Assets/Resources/Controls.txt";
        StreamWriter writer = new StreamWriter(path, false);
        
        ICollection keys = bindings.Keys;
        
        foreach( string k in keys )
        { 
            writer.WriteLine(k+"="+bindings[k]);
        }
        writer.Close();
        ApplyKeyBindings();
    }

    private void SetDefaultKeyBindings()
    {
        string path = "Assets/Resources/Controls.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine("R=KeyCode.D");
        writer.WriteLine("L=KeyCode.A");
        writer.WriteLine("S=KeyCode.Return");
        writer.WriteLine("M=KeyCode.Escape");
        writer.WriteLine("C=KeyCode.W");
        
        ApplyKeyBindings();
    }

}