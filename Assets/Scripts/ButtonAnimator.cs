
using System.Collections;
using System.Threading;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class ButtonAnimator:MonoBehaviour{
    [SerializeField]private float animationTime = 0.3f;
    [SerializeField]private Vector2 hoverScale = new Vector2(1.2f,1.2f);
    [SerializeField]private Vector2 HoverOffset = new Vector2(0,0);
    private Vector2 originalPosition;
    private Vector2 originalScale;
    private Button button;
    private bool isNormalScale = true;
    private bool isScalingDown = false;
    private bool isScalingUp = false;
    private float t = 0f;
    private RectTransform rectTransform;
    void Start()
    {

        button = GetComponent<Button>();
        rectTransform = (RectTransform) transform;
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.position;
    }


    private void Update()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Rect rect =new Rect(((Vector2)rectTransform.position)-(rectTransform.rect.size/2.0f),rectTransform.rect.size);
        if(rect.Contains(mousePos))
        {
            if (!isScalingUp)
            {
                StopAllCoroutines();
                StartCoroutine(ScaleUpRoutine());
            }
        }
        else if(!isNormalScale&&!isScalingDown)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleDownRoutine());
        }
    }


    IEnumerator ScaleUpRoutine()
    {
        isNormalScale = false;
        isScalingUp = true;
        isScalingDown = false;
        while (t < animationTime)
        {
            t += Time.unscaledDeltaTime;
            rectTransform.localScale = new Vector3(Mathf.SmoothStep(originalScale.x,hoverScale.x,t/animationTime),
                Mathf.SmoothStep(originalScale.y,hoverScale.y,t/animationTime),1f);
            rectTransform.position=new Vector3(Mathf.SmoothStep(originalPosition.x,originalPosition.x+HoverOffset.x,t/animationTime),
                Mathf.SmoothStep(originalPosition.y,originalPosition.y+HoverOffset.y,t/animationTime),0f);
            yield return null;
        }
        rectTransform.localScale=hoverScale;
        rectTransform.position=originalPosition+HoverOffset;
        isScalingUp = true;
    }

    IEnumerator ScaleDownRoutine()
    {
        isScalingDown = true;
        isScalingUp = false;
        while (t > 0.0f)
        {
            t -= Time.unscaledDeltaTime;
            rectTransform.localScale = new Vector3(Mathf.SmoothStep(originalScale.x,hoverScale.x,t/animationTime),
                Mathf.SmoothStep(originalScale.y,hoverScale.y,t/animationTime),1f);
            rectTransform.position=new Vector3(Mathf.SmoothStep(originalPosition.x,originalPosition.x+HoverOffset.x,t/animationTime),
                Mathf.SmoothStep(originalPosition.y,originalPosition.y+HoverOffset.y,t/animationTime),0f);
            yield return null;
        }
        rectTransform.localScale=originalScale;
        rectTransform.position=originalPosition;
        isNormalScale = true;
        isScalingDown = false;
    }
}