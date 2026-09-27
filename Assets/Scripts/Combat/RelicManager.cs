using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    [SerializeField] private RelicPoolData actOneRelicPool;
    [SerializeField] private RelicPoolData actTwoRelicPool;
    [SerializeField] private RelicPoolData actThreeRelicPool;
    
    private readonly List<RelicData> _relicCollection  = new();
    private readonly List<RelicData> _currentRelicPool = new();

    public void RunSetup(RunManager.CurrentAct currentAct)
    {
        _currentRelicPool.Clear();
        _relicCollection.Clear();

        LoadRelicPool(currentAct);
    }

    public void LoadRelicPool(RunManager.CurrentAct currentAct)
    {
        switch (currentAct)
        {
            case RunManager.CurrentAct.ActOne:
                foreach (var relic in actOneRelicPool.GetRewardPool())
                {
                    _currentRelicPool.Add(relic);
                }
                break;
            case RunManager.CurrentAct.ActTwo:
                foreach (var relic in actTwoRelicPool.GetRewardPool())
                {
                    _currentRelicPool.Add(relic);
                }
                break;
            case RunManager.CurrentAct.ActThree:
                foreach (var relic in actThreeRelicPool.GetRewardPool())
                {
                    _currentRelicPool.Add(relic);
                }
                break;
        }
    }

    public RelicData SelectRelic()
    {
        if (_currentRelicPool.Count <= 0) return null;
        
        var randomIndex = Random.Range(0, _currentRelicPool.Count);
        var randomRelicToAdd = _currentRelicPool[randomIndex];
        
        AddRelicToCollection(randomRelicToAdd);
        return randomRelicToAdd;
    }
    
    private void AddRelicToCollection(RelicData relic)
    {
        if (relic == null) return;
        if (_relicCollection.Contains(relic)) return;
        
        _relicCollection.Add(relic);

        if (!_currentRelicPool.Contains(relic)) return;
        
        _currentRelicPool.Remove(relic);
    }

    public IReadOnlyList<RelicData> GetRelicCollection()
    {
        return _relicCollection;
    }

    public void TriggerStartOfCombatEffects(Player player, EnemyManager enemyManager)
    {
        if (_relicCollection.Count <= 0) return;
        
        foreach (var relic in _relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.StartOfCombat)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerEndOfCombatEffects(Player player, EnemyManager enemyManager)
    {
        if (_relicCollection.Count <= 0) return;
        
        foreach (var relic in _relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.EndOfCombat)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerAttackThresholdHitEffect(Player player, EnemyManager enemyManager)
    {
        if (_relicCollection.Count <= 0) return;
        
        foreach (var relic in _relicCollection)
        {
            if (relic.GetRelicType() == RelicData.RelicType.Attack)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerCorruptionOverflowEffect(Player player, EnemyManager enemyManager)
    {
        if (_relicCollection.Count <= 0) return;
        
        foreach (var relic in _relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.CorruptionOverflow)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }

    public void TriggerStartOfTurnEffects(Player player, EnemyManager enemyManager)
    {
        if (_relicCollection.Count <= 0) return;
        
        foreach (var relic in _relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.StartOfTurn)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }
    
    public void TriggerCriticalHealthEffects(Player player, EnemyManager enemyManager)
    {
        if (_relicCollection.Count <= 0) return;
        
        foreach (var relic in _relicCollection)
        {
            if (relic.GetTriggerTime() == RelicData.TriggerTime.CriticalHealth)
            {
                relic.ResolveEffect(player, enemyManager);
            }
        }
    }
}
