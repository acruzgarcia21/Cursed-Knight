using UnityEngine.UI;
using UnityEngine;

public class RelicIconSlot : MonoBehaviour, ITooltipProvider
{
    [SerializeField] private Image relicIcon;
    
    private RelicData _relicData;
    
    public void DisplayRelic(RelicData relicData)
    {
        gameObject.SetActive(true);

        relicIcon.sprite = relicData.GetRelicSprite();
        
        _relicData = relicData;
    }

    public void HideRelic()
    {
        relicIcon.sprite = null;
        
        gameObject.SetActive(false);
        
        _relicData = null;
    }
    
    public TooltipData GetTooltipData()
    {
        if (_relicData == null) return null;
        
        var relicName        = _relicData.GetRelicName();
        var relicDescription = _relicData.GetRelicDescription();
        
        return new TooltipData(relicName, relicDescription);
    }
}
