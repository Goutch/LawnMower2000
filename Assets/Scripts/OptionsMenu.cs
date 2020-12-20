using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class OptionsMenu : MonoBehaviour
{
    
    [SerializeField] private int KeyBindingTimeInSeconds;
    [SerializeField] private Text LeftKeyText;
    [SerializeField] private Text MenuKeyText;
    [SerializeField] private Text RightKeyText;
    [SerializeField] private Text ContinueKeyText;
    [SerializeField] private Text StartKeyText;
    [SerializeField] private Button ApplyButton;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;

    private GameManager gameManager;
    private Options options;
    private Event keyEvent;
    private KeyCode pressedKey;
    private bool keyBindingChanged;
    private Hashtable keyBindingText;
    private bool waitingForKey;
    private Coroutine waitingForKey_Coroutine;
    private Coroutine timer_Coroutine;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        keyBindingChanged = false;
        keyBindingText = new Hashtable();
        waitingForKey = false;
        InitKeyBindingText();
        SetAllVolumeSliders();
    }

    public void OnApplyButtonClicked()
    {
        options.ApplyKeyBindings();
        options.ApplyVolumeSettings();
        keyBindingChanged = false;
    }

    public void OnDefaultButtonClicked()
    {
        options.SetDefaultKeyBindings();
        options.SetDefaultVolumeSettings();
        SetAllKeysBindingText();
    }

    public void SetVolume(string key)
    {
        if (key == "E")
        {
            options.SetVolumeLevel(key,SFXVolumeSlider.value);
        }
        else if (key == "M")
        {
            options.SetVolumeLevel(key,MusicVolumeSlider.value);
        }
    }

    private void SetKeyBinding(string key, KeyCode keyCode)
    {
        options.SetKeyBinding(key,keyCode);
        SetKeyBindingTextValue(key);
        keyBindingChanged = true;
    }

    public void OnKeyBindingButtonClicked(string id)
    {
        ApplyButton.enabled = false;
        if (waitingForKey_Coroutine != null)
        {
            StopCoroutine(waitingForKey_Coroutine);
            if (timer_Coroutine != null)
            {
                StopCoroutine(timer_Coroutine);
            }
        }
        
        waitingForKey_Coroutine = StartCoroutine(WaitForKey(id));
    }

    IEnumerator WaitForKey(string id)
    {
        bool keyBindingChanged = false;
        waitingForKey = true;
        timer_Coroutine = StartCoroutine(KeyBindingTimer(id));
        while (waitingForKey)
        {
            if(Input.anyKey)
            {
                foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(k))
                    {
                        pressedKey = k;
                        waitingForKey = false;
                        keyBindingChanged = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        if (keyBindingChanged)
        {
            SetKeyBinding(id,pressedKey);
        }
        else
        {
            SetKeyBindingTextValue(id);
        }

        ApplyButton.enabled = true;
    }

    IEnumerator KeyBindingTimer(string id)
    {
        int timer = KeyBindingTimeInSeconds;
        while (timer > 0 && waitingForKey)
        {
            ((Text) keyBindingText[id]).text = timer.ToString();
            timer--;
            yield return new WaitForSeconds(1);
        }
        waitingForKey = false;
    }

    private void SetKeyBindingTextValue(string key)
    {
       ((Text) keyBindingText[key]).text = options.getKeyBinding(key).ToString();
    }

    private void InitKeyBindingText()
    {
        keyBindingText.Add("L",LeftKeyText);
        keyBindingText.Add("M",MenuKeyText);
        keyBindingText.Add("R", RightKeyText);
        keyBindingText.Add("C", ContinueKeyText);
        keyBindingText.Add("S",StartKeyText);
        SetAllKeysBindingText();
    }

    private void SetAllKeysBindingText()
    {
        foreach (string k in keyBindingText.Keys)
        {
            SetKeyBindingTextValue(k);
        }
    }

    private void SetAllVolumeSliders()
    {
        SFXVolumeSlider.value = options.effectsVolume;
        MusicVolumeSlider.value = options.musicVolume;
    }

    private void OnEnable()
    {
        if (options != null)
        {
            options.ReadKeyBindings();
            options.ReadVolumeSettings();
            SetAllKeysBindingText();
            SetAllVolumeSliders();
        }
    }
    
}
