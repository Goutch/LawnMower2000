using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectionMenu : MonoBehaviour
{
    [SerializeField] private GameObject AvatarSelectionMenuPanel = null;
    [SerializeField] private Image PreviewTexture = null;
    [SerializeField] private ChooseSprite BaseSpriteChosen = null;
    [SerializeField] private ChooseSprite EngineSpriteChosen = null;
    [SerializeField] private Color PrimaryColorBaseReferent = Color.white;
    [SerializeField] private Color SecondaryColorBaseReferent = Color.white;
    [SerializeField] private Color PrimaryColorEnginReferent = Color.white;
    [SerializeField] private Color SecondaryColorEnginReferent = Color.white;
    [SerializeField] private Button PresetButton = null;

    public delegate void OnStartHandler();
    public event OnStartHandler OnStartEvent;

    private void Awake()
    {
        BaseSpriteChosen.OnSpriteChangedEvent += CreateTextures;
        EngineSpriteChosen.OnSpriteChangedEvent += CreateTextures;

        BaseSpriteChosen.OnColorChangedEvent += CreateTextures;
        EngineSpriteChosen.OnColorChangedEvent += CreateTextures;
    }

    private void OnEnable()
    {
        PresetButton.onClick?.Invoke();
    }

    private void OnDestroy()
    {
        BaseSpriteChosen.OnSpriteChangedEvent -= CreateTextures;
        EngineSpriteChosen.OnSpriteChangedEvent -= CreateTextures;

        BaseSpriteChosen.OnColorChangedEvent -= CreateTextures;
        EngineSpriteChosen.OnColorChangedEvent -= CreateTextures;
    }

    private Sprite CreateTexture(int indexBase, int indexEngine, int animIndex)
    {
        int textureWidth = BaseSpriteChosen.GetChosenSprite(indexBase, 0).texture.width;
        int textureHeight = BaseSpriteChosen.GetChosenSprite(indexBase, 0).texture.height;

        Color[] pixelsBase = BaseSpriteChosen.GetChosenSprite(indexBase, 0).texture.GetPixels();
        Color[] pixelEngine = EngineSpriteChosen.GetChosenSprite(indexEngine, animIndex).texture.GetPixels();

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

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f,0.5f), 16);
        sprite.texture.filterMode = FilterMode.Point;
       

        return sprite;
    }

    public void CreateTextures()
    {
        Sprite[] Idles = new Sprite[6];
        for (int i = 0; i < 6; ++i)
        {
            Idles[i] = CreateTexture(0,i,0);
        }

        Sprite[] GotPoints = new Sprite[4];
        for (int i = 0; i < 4; ++i)
        {
            GotPoints[i] = CreateTexture(0, i, 1);
        }

        PreviewTexture.sprite = Idles[0];

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Options.LawnMower1Color = BaseSpriteChosen.GetPrimaryColor();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Options.LawnMower2Color = Color.white;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PlayerSpriteIdle = Idles;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PlayerGotPoints = GotPoints;
    }

    public void OnStartButtonClick()
    {
        OnStartEvent?.Invoke();
    }
}
