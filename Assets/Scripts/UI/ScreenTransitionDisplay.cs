using System.Collections;
using UnityEngine;

public class ScreenTransitionDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float fadeDuration = 0.2f;

    private float _elapsedTime;
    private float _progress;


    private void Awake()
    {
        var overlayImage = GetComponent<UnityEngine.UI.Image>();
        overlayImage.raycastTarget = true;
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeToInvisible()
    {
        canvasGroup.blocksRaycasts = true;
        _elapsedTime = 0;
        _progress = 0;

        var currentAlpha = canvasGroup.alpha;
        
        while (_elapsedTime < fadeDuration)
        {
            UpdateFadeProgress();

            canvasGroup.alpha = Mathf.Lerp(currentAlpha, 0f, _progress);
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    
    public IEnumerator FadeToBlack()
    {
        canvasGroup.blocksRaycasts = true;
        _elapsedTime = 0;
        _progress = 0;
        
        var currentAlpha = canvasGroup.alpha;
        
        while (_elapsedTime < fadeDuration)
        {
            UpdateFadeProgress();

            canvasGroup.alpha = Mathf.Lerp(currentAlpha, 1f, _progress);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    private void UpdateFadeProgress()
    {
        _elapsedTime += Time.deltaTime;
        _progress = fadeDuration > 0f ? Mathf.Clamp01(_elapsedTime / fadeDuration) : 1f;
    }
}
