using UnityEngine;

[System.Serializable]
public class MapNodeSaveData
{
    [SerializeField] private string nodeID;

    [SerializeField] private Map.MapNodeType nodeType;

    public MapNodeSaveData(string nodeID, Map.MapNodeType nodeType)
    {
        this.nodeID   = nodeID;
        this.nodeType = nodeType;
    }
}
