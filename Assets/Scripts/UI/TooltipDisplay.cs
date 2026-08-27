using System;
using TMPro;
using UnityEngine;

public class TooltipDisplay : MonoBehaviour
{
    
    [SerializeField] private GameObject toolTipPanel;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private float tooltipOffsetX = 10f;

    [SerializeField] private RectTransform canvasTransform;
    
    private RectTransform _tooltipPanelRectTransform;

    private void Awake()
    {
        toolTipPanel.SetActive(false);

        _tooltipPanelRectTransform = toolTipPanel.GetComponent<RectTransform>();
    }

    public void ShowTooltip(TooltipData tooltipData, RectTransform sourcePoint)
    {
        titleText.text       = tooltipData.GetTooltipTitle();
        descriptionText.text = tooltipData.GetTooltipDescription();

        var sourcePointInCanvas = canvasTransform.InverseTransformPoint(sourcePoint.position);

        var sourceHalfWidth = sourcePoint.rect.width / 2f;
        var tooltipHalfWidth = _tooltipPanelRectTransform.rect.width / 2f;

        if (sourcePointInCanvas.x < 0)
        {
            var tooltipX = 
                sourcePointInCanvas.x + sourceHalfWidth + tooltipHalfWidth + tooltipOffsetX;
            
            toolTipPanel.transform.localPosition = new Vector3(tooltipX, sourcePointInCanvas.y, 0);
        }
        else
        {
            var tooltipX = 
                sourcePointInCanvas.x - sourceHalfWidth - tooltipHalfWidth - tooltipOffsetX;
            
            toolTipPanel.transform.localPosition = new Vector3(tooltipX, sourcePointInCanvas.y, 0);
        }
        
        toolTipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        toolTipPanel.SetActive(false);
    }
}
