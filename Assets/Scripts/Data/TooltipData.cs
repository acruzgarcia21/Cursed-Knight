using System;
using UnityEngine;

[Serializable]
public class TooltipData
{
    [SerializeField] private string title;
    [SerializeField] private string description;

    public string GetTooltipTitle()
    {
        return title;
    }

    public string GetTooltipDescription()
    {
        return description;
    }
}
