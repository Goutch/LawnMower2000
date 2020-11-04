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
    private GameManager gameManager;
    private Options options;
    private Event keyEvent;
    private KeyCode pressedKey;
    private bool keyBindingChanged;
    
    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        options = gameManager.GetComponent<Options>();
        keyBindingChanged = false;
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
        keyBindingChanged = true;
    }

    public void OnKeyBindingButtonClicked(string id)
    {
        StartCoroutine(waitForKey(id));
    }

    IEnumerator waitForKey(string id)
    {
        int counter = KeyBindingTimeInSeconds;
        bool waitingForKey = true;
        
        while (waitingForKey&&counter>0)
        {
            
            if(Input.anyKey)
            {
                foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(k))
                    {
                        pressedKey = k;
                        waitingForKey = false;
                        counter++;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(1);
            counter--;
        }

        if (counter != 0)
        {
            SetKeyBinding(id,pressedKey);
            
        }
    }
    
}
