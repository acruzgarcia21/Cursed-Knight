using UnityEngine;

public class CardVisualEffects : MonoBehaviour
{
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    
    public void HandleHoverState(RectTransform rectTransform, Vector3 originalScale)
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;
    }

    public void HandleGlowEffect(bool isGlowEffectActive)
    {
        glowEffect.SetActive(isGlowEffectActive);
    }

    public void HandlePlayArrow(bool isPlayArrowActive)
    {
        playArrow.SetActive(isPlayArrowActive);
    }
}
