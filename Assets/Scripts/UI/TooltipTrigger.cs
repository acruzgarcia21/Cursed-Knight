using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipData tooltipData;

    [SerializeField] private TooltipDisplay tooltipDisplay;

    private RectTransform _sourceRectTransform;

    private void Awake()
    {
        _sourceRectTransform = GetComponent<RectTransform>();
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
