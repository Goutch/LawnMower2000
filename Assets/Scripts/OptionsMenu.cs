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

    private GameManager gameManager;
    private Options options;
    private Event keyEvent;
    private KeyCode pressedKey;
    private bool keyBindingChanged;
    private Hashtable keyBindingText;
    private bool waitingForKey;
    private Coroutine waitingForKey_Coroutine;
    private Coroutine timer_Coroutine;
    private string previous_KeyId;
    private Hashtable keyBindingCache;
    
    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        keyBindingChanged = false;
        keyBindingText = new Hashtable();
        waitingForKey = false;
        previous_KeyId = "L";
        keyBindingCache = new Hashtable();
        InitKeyBindingCache();
        InitKeyBindingText();
    }

    public void OnApplyButtonClicked()
    {
        options.ApplyKeyBindings();
        UpdateKeyBindingCache();
        keyBindingChanged = false;
    }

    public void OnDefaultButtonClicked()
    {
        options.SetDefaultKeyBindings();
        UpdateKeyBindingCache();
        SetAllKeysBindingText();
    }

    private void SetKeyBinding(string key, KeyCode keyCode)
    {
        options.SetKeyBinding(key,keyCode);
        SetKeyBindingTextValue(key);
        keyBindingChanged = true;
    }

    public void OnKeyBindingButtonClicked(string id)
    {
        if (waitingForKey_Coroutine != null)
        {
            StopCoroutine(waitingForKey_Coroutine);
            if (timer_Coroutine != null)
            {
                StopCoroutine(timer_Coroutine);
            }
            SetKeyBindingTextValueFromCache(previous_KeyId);
        }

        previous_KeyId = id;
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
                        keyBindingCache[id] = k;
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

    private void SetKeyBindingTextValueFromCache(string key)
    {
        ((Text) keyBindingText[key]).text = keyBindingCache[key].ToString();
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

    private void InitKeyBindingCache()
    {
        keyBindingCache.Add("L",options.getKeyBinding("L"));
        keyBindingCache.Add("M",options.getKeyBinding("M"));
        keyBindingCache.Add("R",options.getKeyBinding("R"));
        keyBindingCache.Add("C",options.getKeyBinding("C"));
        keyBindingCache.Add("S",options.getKeyBinding("S"));
    }

    private void UpdateKeyBindingCache()
    {
        foreach (string k in keyBindingCache.Keys)
        {
            keyBindingCache[k] = options.getKeyBinding(k);
        }
    }

}
