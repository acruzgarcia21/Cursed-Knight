using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntentEntryDisplay : MonoBehaviour, ITooltipProvider
{
    public Image intentIcon;
    public TMP_Text intentText;

    private TooltipData _tooltipData;

    public void DisplayIntent(Sprite icon, string text, TooltipData tooltipData)
    {
        if (icon == null) return;
        gameObject.SetActive(true);

        intentIcon.sprite = icon;
        intentText.text = text;
        _tooltipData = tooltipData;
    }

    public void Clear()
    {
        intentIcon.sprite = null;
        intentText.text = "";
        _tooltipData = null;
        
        gameObject.SetActive(false);
    }
    
    public TooltipData GetTooltipData()
    {
        return _tooltipData;
    }
}
