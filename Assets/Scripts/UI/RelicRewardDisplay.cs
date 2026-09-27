using UnityEngine;

public class RelicRewardDisplay : MonoBehaviour, ITooltipProvider
{
    private RelicData _relicData;
    public void DisplayRelic(RelicData relicData)
    {
        if (relicData == null) return;
        _relicData = relicData;
    }
    public TooltipData GetTooltipData()
    {
        if (_relicData == null) return null;
        
        var relicName        = _relicData.GetRelicName();
        var relicDescription = _relicData.GetRelicDescription();
        
        return new TooltipData(relicName, relicDescription);
    }
}
