using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        ReadKeyBindings();
    }

    public void ReadKeyBindings()
    {
        string path = "Assets/Resources/Controls.txt";
        StreamReader reader = new StreamReader(path);
        string line; 
        bindings = new Hashtable();
        
        while ((line = reader.ReadLine()) != null)
        {
            string[] txtLine = line.Split('=');
            bindings.Add(txtLine[0], System.Enum.Parse(typeof(KeyCode), txtLine[1]) );
        }
        
        Debug.Log(reader.ReadToEnd());
        reader.Close();

        if (bindings.Count < AMOUNT_OF_KEY_BINDINGS)
        {
            SetDefaultKeyBindings();
        }
        else
        {
            ApplyKeyBindings();
        }
    }

    public void ApplyKeyBindings()
    {
        TurnRightKey = (KeyCode)bindings["R"];
        TurnLeftKey = (KeyCode) bindings["L"];
        ReadyKey = (KeyCode) bindings["S"];
        InGameMenuButtonKey = (KeyCode) bindings["M"];
        ContinueKey = (KeyCode) bindings["C"];
        
        WriteKeyBindings();
    }

    public void SetKeyBinding(string key, KeyCode keyCode)
    {
        bindings[key] = keyCode;
    }

    private void WriteKeyBindings()
    {
        string path = "Assets/Resources/Controls.txt";
        StreamWriter writer = new StreamWriter(path, false);
        
        ICollection keys = bindings.Keys;
        
        foreach( string k in keys )
        { 
            writer.WriteLine(k+"="+bindings[k]);
        }
        writer.Close();
    }

    public void SetDefaultKeyBindings()
    {
        string path = "Assets/Resources/Controls.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine("R=D");
        writer.WriteLine("L=A");
        writer.WriteLine("S=Return");
        writer.WriteLine("M=Escape");
        writer.WriteLine("C=W");
        writer.Close();
        
        ReadKeyBindings();
    }

}