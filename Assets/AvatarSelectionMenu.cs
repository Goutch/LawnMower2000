﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectionMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject AvatarSelectionMenuPanel = null;
    [SerializeField] private Image PreviewTexture = null;
    [SerializeField] private ChooseSprite BaseSpriteChosen = null;
    [SerializeField] private ChooseSprite EngineSpriteChosen = null;
    [SerializeField] private Color PrimaryColorBaseReferent = Color.white;
    [SerializeField] private Color SecondaryColorBaseReferent = Color.white;
    [SerializeField] private Color PrimaryColorEnginReferent = Color.white;
    [SerializeField] private Color SecondaryColorEnginReferent = Color.white;

    public delegate void OnStartHandler();
    public event OnStartHandler OnStartEvent;

    public void OnBackButtonClick()
    {
        MainMenuPanel.SetActive(true);
        AvatarSelectionMenuPanel.SetActive(false);
    }

    private void Awake()
    {
        BaseSpriteChosen.OnSpriteChangedEvent += CreateTexture;
        EngineSpriteChosen.OnSpriteChangedEvent += CreateTexture;

        BaseSpriteChosen.OnColorChangedEvent += CreateTexture;
        EngineSpriteChosen.OnColorChangedEvent += CreateTexture;
    }

    private void OnDestroy()
    {
        BaseSpriteChosen.OnSpriteChangedEvent -= CreateTexture;
        EngineSpriteChosen.OnSpriteChangedEvent -= CreateTexture;

        BaseSpriteChosen.OnColorChangedEvent -= CreateTexture;
        EngineSpriteChosen.OnColorChangedEvent -= CreateTexture;
    }


    private void CreateTexture()
    {
        int textureWidth = BaseSpriteChosen.GetChosenSprite().texture.width;
        int textureHeight = BaseSpriteChosen.GetChosenSprite().texture.height;

        Color[] pixelsBase = BaseSpriteChosen.GetChosenSprite().texture.GetPixels();
        Color[] pixelEngine = EngineSpriteChosen.GetChosenSprite().texture.GetPixels();

        Color[] pixelsResult = new Color[pixelsBase.Length];

        Color basePrimaryColor = BaseSpriteChosen.GetPrimaryColor();
        Color baseSecondaryColor = BaseSpriteChosen.GetSecondaryColor();
        Color enginPrimaryColor = EngineSpriteChosen.GetPrimaryColor();
        Color enginSecondaryColor = EngineSpriteChosen.GetSecondaryColor();

        for (int i = 0; i < pixelsBase.Length; ++i)
        {
            if (pixelEngine[i].a != 0)
            {
                if(pixelEngine[i] == PrimaryColorEnginReferent)
                {
                    pixelsResult[i] = enginPrimaryColor;
                }
                else if(pixelEngine[i] == SecondaryColorEnginReferent)
                {
                    pixelsResult[i] = enginSecondaryColor;
                }
                else
                {
                    pixelsResult[i] = pixelEngine[i];
                }
            }
            else
            {
                if (pixelsBase[i] == PrimaryColorBaseReferent)
                {
                    pixelsResult[i] = basePrimaryColor;
                }
                else if (pixelsBase[i] == SecondaryColorBaseReferent)
                {
                    pixelsResult[i] = baseSecondaryColor;
                }
                else
                {
                    pixelsResult[i] = pixelsBase[i];
                }
            }
        }

        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.SetPixels(pixelsResult);

        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), Vector2.zero, 16);
        sprite.texture.filterMode = FilterMode.Point;
        PreviewTexture.sprite = sprite;
    }

    public void OnStartButtonClick()
    {
        OnStartEvent?.Invoke();
    }
}