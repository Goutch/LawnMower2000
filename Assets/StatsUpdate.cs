using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUpdate : MonoBehaviour
{
    [SerializeField] private Vector2 Offset = new Vector2(20, -20);
    [SerializeField] private float Spacing = 20;

    public LawnMower LawnMower { get; set; } = null;
    public int Position { get; set; } = 0;

    private Text pointsText = null;
    private Image lawnMowerColorImage = null;

    private void OnEnable()
    {
        pointsText = GetComponentInChildren<Text>();
        lawnMowerColorImage = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        transform.localPosition = new Vector2(0, -(Spacing + lawnMowerColorImage.rectTransform.rect.height) * Position) + Offset;
        lawnMowerColorImage.color = LawnMower.Color;
        pointsText.text = ":" + LawnMower.GetPoints();
    }
}
