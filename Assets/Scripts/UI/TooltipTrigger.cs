using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipData tooltipData;

    [SerializeField] private TooltipDisplay tooltipDisplay;

    [SerializeField] private RectTransform tooltipAnchor;

    private RectTransform _sourceRectTransform;

    private void Awake()
    {
        if (tooltipAnchor != null)
        {
            _sourceRectTransform = tooltipAnchor;
        }
        else
        {
            _sourceRectTransform = GetComponent<RectTransform>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipDisplay.ShowTooltip(tooltipData, _sourceRectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipDisplay.HideTooltip();
    }
}
