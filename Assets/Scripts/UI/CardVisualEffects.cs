using UnityEngine;

public class CardVisualEffects : MonoBehaviour
{
    [SerializeField] private float selectScale = 2f;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    
    public void HandleHoverState(RectTransform rectTransform, Vector3 originalScale, float lerpFactor)
    {
        glowEffect.SetActive(true);
        
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale, 
            originalScale * selectScale, 
            lerpFactor * Time.deltaTime);
    }

    public void HandleScaleToNormal(RectTransform rectTransform, Vector3 originalScale, float lerpFactor)
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale, 
            originalScale, 
            lerpFactor * Time.deltaTime);
    }

    public void HandleGlowEffect(bool isGlowEffectActive)
    {
        glowEffect.SetActive(isGlowEffectActive);
    }

    public void ShowPlayArrow(bool isPlayArrowActive)
    {
        playArrow.SetActive(isPlayArrowActive);
    }
}
