using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusDisplay : MonoBehaviour
{
    public List<StatusIconsSlot> statusIconsSlots;
    public List<StatusDisplayData> displayData;
    public StatusManager statusManager;

    private void Awake()
    {
        statusManager = GetComponent<StatusManager>();
    }

    public void OnEnable()
    {
        if (statusManager == null) return;
        
        statusManager.OnStatusesChanged += Refresh;
        Refresh();
    }

    public void OnDisable()
    {
        statusManager.OnStatusesChanged -= Refresh;
    }

    private void Refresh()
    {
        var activeStatuses = statusManager.GetActiveStatuses();

        Debug.Log(
            $"{name} StatusDisplay Refresh | " +
            $"Manager: {statusManager.gameObject.name} | " +
            $"Active statuses: {activeStatuses.Count}"
        );

        ClearAllSlots();

        var slotIndex = 0;

        foreach (var status in activeStatuses)
        {
            if (slotIndex >= statusIconsSlots.Count)
                break;

            var matchingDisplayData = FindDisplayData(status.statusType);

            if (matchingDisplayData == null)
                continue;

            statusIconsSlots[slotIndex].DisplayStatus(status, matchingDisplayData);
            slotIndex++;
        }
    }

    private StatusDisplayData FindDisplayData(StatusEffect.StatusType statusType)
    {
        foreach (var data in displayData)
        {
            if (data.statusType != statusType) continue;

            return data;
        }

        return null;
    }

    private void ClearAllSlots()
    {
        foreach (var icon in statusIconsSlots)
        {
            icon.Clear();
        }
    }
}
