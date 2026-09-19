using System;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public Map.MapNodeType nodeType;
    
    public List<MapNode> nextNodes;

    [SerializeField] private bool randomizeNodeType;
    
    public bool CanRandomizeNodeType()
    {
        return randomizeNodeType;
    }
    
}

[Serializable]
public class RandomNodeType
{
    [SerializeField] private Map.MapNodeType nodeType;
    [SerializeField] private int selectionWeight;

    public int GetSelectionWeight()
    {
        return selectionWeight;
    }

    public Map.MapNodeType GetNodeType()
    {
        return nodeType;
    }
}
