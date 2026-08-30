using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconsSlot : MonoBehaviour, ITooltipProvider
{
    public Image statusIcon;
    
    public TMP_Text countText;
    
    public StatusEffect.StatusType statusType;
    
    public StatusDisplayData statusDisplayData;

    private StatusEffect _statusEffect;

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
        _statusEffect = status;
    }

    public void Clear()
    {
        statusIcon.sprite = null;
        countText.text = "";
        statusType = default;
        statusDisplayData = null;
        _statusEffect = null;
        
        gameObject.SetActive(false);
    }
    
    public TooltipData GetTooltipData()
    {
        if (_statusEffect == null || statusDisplayData == null) return null;

        return statusType switch
        {
            StatusEffect.StatusType.Strength => new TooltipData(statusDisplayData.displayName,
                "Increases damage dealt by " + _statusEffect.amount + "."),
            
            StatusEffect.StatusType.Weak => new TooltipData(statusDisplayData.displayName,
                "Decreases damage dealt by " 
                        + _statusEffect.amount 
                        + " for " 
                        + _statusEffect.duration 
                        + " turns."),
            
            StatusEffect.StatusType.Vulnerable => new TooltipData(statusDisplayData.displayName,
                "Increases damage taken by 50% for " + _statusEffect.duration + " turns."),
            
            StatusEffect.StatusType.Poison => new TooltipData(statusDisplayData.displayName,
                "Takes " 
                        + _statusEffect.amount 
                        + " damage at the end of each turn for " 
                        + _statusEffect.duration 
                        + " turns."),
            
            StatusEffect.StatusType.Bleed => new TooltipData(statusDisplayData.displayName,
                "Takes " 
                        + _statusEffect.amount 
                        + " damage whenever performing an action for " 
                        + _statusEffect.duration 
                        + " turns."),
            _ => null
        };
    }
}
