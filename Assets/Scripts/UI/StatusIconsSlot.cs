using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconsSlot : MonoBehaviour
{
    public Image statusIcon;
    public TMP_Text countText;
    public StatusEffect.StatusType statusType;
    public StatusDisplayData statusDisplayData;

    public void DisplayStatus(StatusEffect status, StatusDisplayData displayData)
    {
        if (displayData == null) return;
        
        gameObject.SetActive(true);
        
        statusIcon.sprite = displayData.icon;
        countText.text = displayData.countDisplayType switch
        {
            StatusDisplayData.StatusCountDisplayType.Amount => status.amount.ToString(),
            StatusDisplayData.StatusCountDisplayType.Duration => status.duration.ToString(),
            _ => ""
        };
        
        statusType = status.statusType;
        statusDisplayData = displayData;
    }

    public void Clear()
    {
        statusIcon.sprite = null;
        countText.text = "";
        statusType = default;
        statusDisplayData = null;
        
        gameObject.SetActive(false);
    }
}
