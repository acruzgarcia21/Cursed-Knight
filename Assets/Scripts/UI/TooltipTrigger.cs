using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TooltipData tooltipData;

    [SerializeField] private TooltipDisplay tooltipDisplay;

    [SerializeField] private RectTransform tooltipAnchor;

    private RectTransform _sourceRectTransform;

    private ITooltipProvider _tooltipProvider;

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

        _tooltipProvider = GetComponentInParent<ITooltipProvider>();

        if (tooltipDisplay == null)
        {
            tooltipDisplay = FindFirstObjectByType<TooltipDisplay>(FindObjectsInactive.Include);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipProvider != null)
        {
            var tooltipProviderData = _tooltipProvider.GetTooltipData();

            if (tooltipProviderData == null) return;
            
            tooltipDisplay.ShowTooltip(tooltipProviderData, _sourceRectTransform);
        }
        else
        {
            tooltipDisplay.ShowTooltip(tooltipData, _sourceRectTransform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipDisplay.HideTooltip();
    }
}
