using System;
using TMPro;
using UnityEngine;

public class TooltipDisplay : MonoBehaviour
{
    [SerializeField] private GameObject toolTipPanel;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        toolTipPanel.SetActive(false);
    }

    public void ShowTooltip(TooltipData tooltipData)
    {
        titleText.text       = tooltipData.GetTooltipTitle();
        descriptionText.text = tooltipData.GetTooltipDescription();
        
        toolTipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        toolTipPanel.SetActive(false);
    }
}
