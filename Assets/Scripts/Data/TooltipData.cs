using System;
using UnityEngine;

[Serializable]
public class TooltipData
{
    [SerializeField] private string title;
    [SerializeField] private string description;

    public TooltipData(string title, string description)
    {
        this.title       = title;
        this.description = description;
    }

    public string GetTooltipTitle()
    {
        return title;
    }

    public string GetTooltipDescription()
    {
        return description;
    }
}
