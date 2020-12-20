using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SpritesArray
{
    [SerializeField] public Sprite[] SpritesIdle;
    [SerializeField] public Sprite[] SpritesGotPoints;
}

public class ChooseSprite : MonoBehaviour
{
    [SerializeField] public SpritesArray[] Sprites;
    [SerializeField] private Image Image = null;
    [SerializeField] private Text Text = null;
    [SerializeField] private Button Previousbutton = null;
    [SerializeField] private Button NextButton = null;
    [SerializeField] private Color[] Colors = null;

    public delegate void OnSpriteChangedHandler();
    public event OnSpriteChangedHandler OnSpriteChangedEvent;

    public delegate void OnColorChangedHandler();
    public event OnColorChangedHandler OnColorChangedEvent;

    public int currentIndex = 0;
    private int currentPrimaryColor = 0;
    private int currentSecondaryColor = 0;

    public SpritesArray GetCurrentSprite()
    {
        return Sprites[currentIndex];
    }

    public Sprite GetChosenSprite(int indexAnim, int animIndex)
    {
        if (animIndex == 0)
            return Sprites[currentIndex].SpritesIdle[indexAnim];
        else
            return Sprites[currentIndex].SpritesGotPoints[indexAnim];
    }

    public Color GetPrimaryColor()
    {
        return Colors[currentPrimaryColor];
    }

    public Color GetSecondaryColor()
    {
        return Colors[currentSecondaryColor];
    }

    private void Start()
    {
        if(Sprites.Length > 0)
        {
            ChangeImage(0);
        }
    }

    public void ChangeImage(int index)
    {
        if(index == 0)
        {
            Previousbutton.interactable = false;
        }
        else
        {
            Previousbutton.interactable = true;
        }

        if(index == Sprites.Length - 1)
        {
            NextButton.interactable = false;
        }
        else
        {
            NextButton.interactable = true;
        }

        Text.text = Sprites[index].SpritesIdle[0].name;
        Image.sprite = Sprites[index].SpritesIdle[0];
        currentIndex = index;

        OnSpriteChangedEvent?.Invoke();
    }

    public void OnNextButtonClick()
    {
        ChangeImage((currentIndex + 1) % Sprites.Length);
    }

    public void OnPreviousButtonClick()
    {
        ChangeImage(currentIndex - 1 < 0 ? Sprites.Length - 1 : currentIndex - 1);
    }

    public void OnColorChoosePrimary(int indexColor)
    {
        currentPrimaryColor = indexColor;
        OnColorChangedEvent?.Invoke();
    }

    public void OnColorChooseSecondary(int indexColor)
    {
        currentSecondaryColor = indexColor;
        OnColorChangedEvent?.Invoke();
    }
}
