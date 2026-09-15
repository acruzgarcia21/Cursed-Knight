using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private List<MapNode> allMapNodes;
    
    [SerializeField] private MapNode startingMapNode;
    
    // private MapNode _bossNode;
    
    public enum MapNodeType
    {
        Battle,
        Rest,
        Elite,
        Boss
    }

    public IReadOnlyList<MapNode> GetAllMapNodes()
    {
        return allMapNodes;
    }
    
}
