using System.Collections.Generic;
using UnityEngine;

public class RelicDisplay : MonoBehaviour
{
    [SerializeField] private List<RelicIconSlot> relicIconsSlots;
    
    private RelicManager _relicManager;

    private void Awake()
    {
        _relicManager = FindAnyObjectByType<RelicManager>();
    }
    
    public void OnEnable()
    {
        if (_relicManager == null) return;
        
        _relicManager.OnRelicsChanged += Refresh;
        Refresh();
    }

    public void OnDisable()
    {
        _relicManager.OnRelicsChanged -= Refresh;
    }

    private void Refresh()
    {
        var relicCollection = _relicManager.GetRelicCollection();
        
        ClearAllSlots();

        var slotIndex = 0;

        foreach (var relic in relicCollection)
        {
            if (slotIndex >= relicIconsSlots.Count) break;
            
            relicIconsSlots[slotIndex].DisplayRelic(relic);
            slotIndex++;
        }
    }

    private void ClearAllSlots()
    {
        foreach (var relic in relicIconsSlots)
        {
            relic.HideRelic();
        }
    }
}
