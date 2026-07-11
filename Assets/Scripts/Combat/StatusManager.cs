using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public event Action OnStatusesChanged;
    
    private readonly List<StatusEffect> _activeStatuses = new();

    public void ApplyStatus(StatusEffect statusEffect)
    {
        if (statusEffect == null) return;

        foreach (var activeStatus in _activeStatuses)
        {
            if (activeStatus.statusType != statusEffect.statusType) continue;
            
            activeStatus.amount += statusEffect.amount;
            activeStatus.duration += statusEffect.duration;
            OnStatusesChanged?.Invoke();
            return;
        }
        
        _activeStatuses.Add(statusEffect);
        OnStatusesChanged?.Invoke();
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
        OnStatusesChanged?.Invoke();
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
        OnStatusesChanged?.Invoke();
    }

    public int GetStatusDuration(StatusEffect.StatusType statusType)
    {
        foreach (var activeStatus in _activeStatuses)
        {
            if (activeStatus.statusType != statusType) continue;
            
            return activeStatus.duration;
        }

        return 0;
    }
    
    public IReadOnlyList<StatusEffect> GetActiveStatuses()
    {
        return _activeStatuses;
    }
    
    public void DebugPrintStatuses()
    {
        if (_activeStatuses.Count <= 0)
        {
            Debug.Log("No Active Statuses.");
            return;
        }

        Debug.Log("=== Active Statuses ===");

        foreach (var status in _activeStatuses)
        {
            Debug.Log(
                $"Status: {status.statusType} | " +
                $"Amount: {status.amount} | " +
                $"Duration: {status.duration}"
            );
        }
    }
}
