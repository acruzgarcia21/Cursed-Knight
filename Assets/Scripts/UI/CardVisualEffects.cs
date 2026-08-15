using UnityEngine;
using UnityEngine.Rendering;

public class CardVisualEffects : MonoBehaviour
{
    [SerializeField] private float selectScale = 5f;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    [SerializeField] private float hoverBottomPadding = 1f;
    
    public void HandleHoverState(RectTransform rectTransform, Vector3 originalScale, float lerpFactor)
    {
        glowEffect.SetActive(true);
        
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale, 
            originalScale * selectScale, 
            lerpFactor * Time.deltaTime);
    }

    public void HandleHoverPosition(RectTransform rectTransform, RectTransform canvasRectTransform, float lerpFactor)
    {
        var worldCorners = new Vector3[4];
        
        rectTransform.GetWorldCorners(worldCorners);

        // Center of the bottom of the card
        var averageOfBottomWorldCorner = (worldCorners[0] + worldCorners[3]) / 2;
        
        // Where is the center of the card according to the canvas
        var cardCenterCanvasPosition =
            canvasRectTransform.InverseTransformPoint(rectTransform.position);

        // Where is the bottom of the card according to the canvas
        var cardBottomCanvasPosition =
            canvasRectTransform.InverseTransformPoint(averageOfBottomWorldCorner);
        
        // Distance between bottom and center
        var halfCardHeight = cardCenterCanvasPosition.y - cardBottomCanvasPosition.y;

        var canvasHalfHeight = canvasRectTransform.rect.height / 2f;
        var canvasBottomY = -canvasHalfHeight;

        var targetY = canvasBottomY + halfCardHeight + hoverBottomPadding;

        // Where we want the canvas to say the card is
        var canvasTargetPosition = new Vector3(cardCenterCanvasPosition.x, targetY, cardCenterCanvasPosition.z);

        var worldTargetPosition = canvasRectTransform.TransformPoint(canvasTargetPosition);
        
        var localTargetPosition = rectTransform.parent.InverseTransformPoint(worldTargetPosition);
        
        rectTransform.localPosition = Vector3.Lerp(
            rectTransform.localPosition,
            localTargetPosition,
            lerpFactor * Time.deltaTime
        );
    }

    public void HandleScaleToNormal(RectTransform rectTransform, Vector3 originalScale, float lerpFactor)
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale, 
            originalScale, 
            lerpFactor * Time.deltaTime);
    }

    public void HandleRotationToUpright(RectTransform rectTransform, float lerpFactor)
    {
        var currentRotation = rectTransform.localRotation;
        
        rectTransform.localRotation =  Quaternion.Lerp(
            currentRotation,
            Quaternion.identity, 
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
