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

    public void Refresh()
    {
        var activeStatuses = statusManager.GetActiveStatuses();

        ClearAllSlots();

        for (var i = 0; i < activeStatuses.Count; i++)
        {
            if (i >= statusIconsSlots.Count)
                break;

            StatusEffect status = activeStatuses[i];

            StatusDisplayData matchingDisplayData = FindDisplayData(status.statusType);

            if (matchingDisplayData == null)
                continue;

            statusIconsSlots[i].DisplayStatus(status, matchingDisplayData);
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
