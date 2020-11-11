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
    
    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        keyBindingChanged = false;
        keyBindingText = new Hashtable();
        waitingForKey = false;
        initKeyBindingText();
    }

    public void OnApplyButtonClicked()
    {
        options.ApplyKeyBindings();
        keyBindingChanged = false;
    }

    public void OnDefaultButtonClicked()
    {
        options.SetDefaultKeyBindings();
    }

    private void SetKeyBinding(string key, KeyCode keyCode)
    {
        options.SetKeyBinding(key,keyCode);
        setKeyBindingTextValue(key);
        keyBindingChanged = true;
    }

    public void OnKeyBindingButtonClicked(string id)
    {
       StartCoroutine(waitForKey(id));
    }

    IEnumerator waitForKey(string id)
    {
        bool keyBindingChanged = false;
        waitingForKey = true;
        StartCoroutine(keyBindingTimer(id));
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
            setKeyBindingTextValue(id);
        }
    }

    IEnumerator keyBindingTimer(string id)
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

    private void setKeyBindingTextValue(string key)
    {
       ((Text) keyBindingText[key]).text = options.getKeyBinding(key).ToString();
    }

    private void initKeyBindingText()
    {
        keyBindingText.Add("L",LeftKeyText);
        keyBindingText.Add("M",MenuKeyText);
        keyBindingText.Add("R", RightKeyText);
        keyBindingText.Add("C", ContinueKeyText);
        keyBindingText.Add("S",StartKeyText);
        foreach (string k in keyBindingText.Keys)
        {
            setKeyBindingTextValue(k);
        }
    }

}
