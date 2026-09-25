using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    [SerializeField] private List<RelicData> relicCollection;

    public void AddRelicToCollection(RelicData relic)
    {
        if (relic == null) return;
        if (relicCollection.Contains(relic)) return;
        
        relicCollection.Add(relic);
    }

    public IReadOnlyList<RelicData> GetRelicCollection()
    {
        return relicCollection;
    }

    public void TriggerStartOfCombatEffects(Player player)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.StartOfCombat)
            {
                relic.ResolveEffect(player);
            }
        }
    }

    public void TriggerEndOfCombatEffects(Player player)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.EndOfCombat)
            {
                relic.ResolveEffect(player);
            }
        }
    }
}
