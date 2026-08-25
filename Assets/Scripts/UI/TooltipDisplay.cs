using System;
using TMPro;
using UnityEngine;

public class TooltipDisplay : MonoBehaviour
{
    [SerializeField] private GameObject toolTipPanel;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private float tooltipOffsetX = 100f;

    private void Awake()
    {
        toolTipPanel.SetActive(false);
    }

    public void ShowTooltip(TooltipData tooltipData, RectTransform sourcePoint)
    {
        titleText.text       = tooltipData.GetTooltipTitle();
        descriptionText.text = tooltipData.GetTooltipDescription();
        
        var tooltipX = sourcePoint.transform.localPosition.x + tooltipOffsetX;

        toolTipPanel.transform.localPosition = new Vector3(tooltipX, sourcePoint.transform.localPosition.y, 0);
        
        toolTipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        toolTipPanel.SetActive(false);
    }
}
