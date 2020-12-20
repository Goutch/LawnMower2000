using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Options : MonoBehaviour
{
    private const string CONTROLS_PATH = "./Controls.json";
    private const string VOLUME_PATH = "./Volume.json";
    
    public delegate void VolumeChangeHandler();

    public event VolumeChangeHandler volumeChanged;

    public enum GameModeType
    {
        OfflineVsAI,
        Online,
    }

    public KeyBinding Controls { private set; get; }
    public VolumeSettings VolumeLevels { private set; get; }
    public GameModeType GameMode { set; get; } = GameModeType.OfflineVsAI;
    public Color LawnMower1Color  = Color.blue;
    public Color LawnMower2Color  = Color.red;
    public Vector2Int MapSize { private set; get; } = new Vector2Int(16, 16);

    public float musicVolume { private set; get; }
    public float effectsVolume { private set; get; }
    public float lawnMowerVolume { private set; get; }


    private Dictionary<string, KeyCode> temporaryBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, float> temporaryVolumeLevels = new Dictionary<string, float>();

    private void Start()
    {
        ReadKeyBindings();
        ReadVolumeSettings();
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

    public void ApplyVolumeSettings()
    {
        VolumeLevels.SoundFX = temporaryVolumeLevels["E"];
        VolumeLevels.Music = temporaryVolumeLevels["M"];
        VolumeLevels.LawnMower = temporaryVolumeLevels["L"];
        WriteVolumeSettings();
        musicVolume = temporaryVolumeLevels["M"];
        effectsVolume = temporaryVolumeLevels["E"];
        lawnMowerVolume = temporaryVolumeLevels["L"];
        volumeChanged?.Invoke();
    }

    public void SetKeyBinding(string key, KeyCode keyCode)
    {
        temporaryBindings[key] = keyCode;
    }

    public void SetVolumeLevel(string key, float volume)
    {
        if (volume < 0 || volume > 1)
        {
            volume = 1;
        }

        temporaryVolumeLevels[key] = volume;
    }

    private void WriteKeyBindings()
    {
        string json = JsonUtility.ToJson(Controls);
        StreamWriter writer = new StreamWriter(CONTROLS_PATH, false);
        writer.Write(json);
        writer.Close();
    }

    private void WriteVolumeSettings()
    {
        string json = JsonUtility.ToJson(VolumeLevels);
        StreamWriter writer = new StreamWriter(VOLUME_PATH,false);
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

    public void ReadVolumeSettings()
    {
        if (File.Exists(VOLUME_PATH))
        {
            StreamReader reader = new StreamReader(VOLUME_PATH);
            VolumeLevels = JsonUtility.FromJson<VolumeSettings>(reader.ReadToEnd());
            reader.Close();
        }
        else
        {
           VolumeLevels = new VolumeSettings();
        }

        temporaryVolumeLevels["E"] = VolumeLevels.SoundFX;
        temporaryVolumeLevels["M"] = VolumeLevels.Music;
        temporaryVolumeLevels["L"] = VolumeLevels.LawnMower;
        musicVolume = temporaryVolumeLevels["M"];
        effectsVolume = temporaryVolumeLevels["E"];
        lawnMowerVolume = temporaryVolumeLevels["L"];
        volumeChanged?.Invoke();
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

    public void SetDefaultVolumeSettings()
    {
        VolumeLevels = new VolumeSettings();
        temporaryVolumeLevels["E"] = VolumeLevels.SoundFX;
        temporaryVolumeLevels["M"] = VolumeLevels.Music;
        temporaryVolumeLevels["L"] = VolumeLevels.LawnMower;
    }

    public KeyCode getKeyBinding(string key)
    {
        return temporaryBindings[key];
    }

    public float getVolumeLevel(string key)
    {
        return temporaryVolumeLevels[key];
    }
}