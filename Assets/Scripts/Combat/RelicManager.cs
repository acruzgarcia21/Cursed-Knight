using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    [SerializeField] private List<RelicData> actOneRelicPool;
    [SerializeField] private List<RelicData> actTwoRelicPool;
    [SerializeField] private List<RelicData> actThreeRelicPool;
    
    private List<RelicData> relicCollection  = new();
    private List<RelicData> currentRelicPool = new();

    public void RunSetup()
    {
        currentRelicPool.Clear();
        relicCollection.Clear();

        foreach (var relic in actOneRelicPool)
        {
            currentRelicPool.Add(relic);
        }
    }
    
    public void AddRelicToCollection(RelicData relic)
    {
        if (relic == null) return;
        if (relicCollection.Contains(relic)) return;
        
        relicCollection.Add(relic);

        if (!currentRelicPool.Contains(relic)) return;
        
        currentRelicPool.Remove(relic);
    }

    public IReadOnlyList<RelicData> GetRelicCollection()
    {
        return relicCollection;
    }

    public void TriggerStartOfCombatEffects(Player player, EnemyManager enemyManager)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.StartOfCombat)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerEndOfCombatEffects(Player player, EnemyManager enemyManager)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.EndOfCombat)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerAttackThresholdHitEffect(Player player, EnemyManager enemyManager)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetRelicType() == RelicData.RelicType.Attack)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerCorruptionOverflowEffect(Player player, EnemyManager enemyManager)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.CorruptionOverflow)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerStartOfTurnEffects(Player player, EnemyManager enemyManager)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.StartOfTurn)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }
    
    public void TriggerCriticalHealthEffects(Player player, EnemyManager enemyManager)
    {
        if (relicCollection.Count <= 0) return;
        
        foreach (var relic in relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.CriticalHealth)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }
}
