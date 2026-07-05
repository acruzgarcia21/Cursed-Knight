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

    public void RemoveStatus(StatusEffect.StatusType statusType)
    {
        StatusEffect statusToRemove = null;
        
        foreach (var activeStatus in _activeStatuses)
        {
            if (activeStatus.statusType != statusType) continue;
            
            statusToRemove = activeStatus;
            break;
        }

        if (statusToRemove == null) return;
        _activeStatuses.Remove(statusToRemove);
    }

    public bool HasStatus(StatusEffect.StatusType statusType)
    {
        foreach (var activeStatus in _activeStatuses)
        {
            if (activeStatus.statusType != statusType) continue;
            
            return true;
        }
        
        return false;
    }

    public int GetStatusAmount(StatusEffect.StatusType statusType)
    {
        foreach (var activeStatus in _activeStatuses)
        {
            if (activeStatus.statusType != statusType) continue;
            
            return activeStatus.amount;
        }

        return 0;
    }

    public void TickDurations()
    {
        for (var i = _activeStatuses.Count - 1; i >= 0; i--)
        {
            var status = _activeStatuses[i];
            
            if (status.duration == -1) continue;

            status.duration--;

            if (status.duration <= 0)
            {
                _activeStatuses.Remove(status);
            }
        }
    }
}
