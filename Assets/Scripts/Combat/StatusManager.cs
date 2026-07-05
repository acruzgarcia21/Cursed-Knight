using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private readonly List<StatusEffect> _activeStatuses = new();

    public void ApplyStatus(StatusEffect statusEffect)
    {
        if (statusEffect == null) return;

        foreach (var activeStatus in _activeStatuses)
        {
            if (activeStatus.statusType != statusEffect.statusType) continue;
            
            activeStatus.amount += statusEffect.amount;
            activeStatus.duration += statusEffect.duration;
            return;
        }
        
        _activeStatuses.Add(statusEffect);
    }

    public void RemoveStatus()
    {
        
    }

    public bool HasStatus()
    {
        return false;   
    }

    public int GetStatusAmount()
    {
        return 0;
    }
}
