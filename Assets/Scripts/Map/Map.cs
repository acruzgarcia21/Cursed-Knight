using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private List<MapNode> allMapNodes;
    
    [SerializeField] private MapNode startingMapNode;
    
    // private MapNode _bossNode;
    [SerializeField] private List<RandomNodeType> randomNodeTypes;
    
    public enum MapNodeType
    {
        Battle,
        Rest,
        Elite,
        Boss,
        None
    }

    public IReadOnlyList<MapNode> GetAllMapNodes()
    {
        return allMapNodes;
    }

    public  MapNode GetStartingMapNode()
    {
        return startingMapNode; 
    }

    public MapNodeType SelectRandomNodeType()
    {
        // Calculate the total weight of the pool
        var totalWeight = 0;

        foreach (var randomNodeType in randomNodeTypes)
        {
            if (randomNodeType.GetSelectionWeight() <= 0) continue;
            
            totalWeight += randomNodeType.GetSelectionWeight();
        }

        if (totalWeight <= 0)
        {
            Debug.Log("There are no valid weighted node types!");
            return MapNodeType.None;
        }
        
        var roll = Random.Range(0, totalWeight);
        var runningWeight = 0;
        
        // Roll somewhere inside the total weight
        foreach (var randomNodeType in randomNodeTypes)
        {
            if (randomNodeType.GetSelectionWeight() <= 0) continue;
            runningWeight += randomNodeType.GetSelectionWeight();

            if (roll >= runningWeight) continue;
            
            return randomNodeType.GetNodeType();
        }

        return MapNodeType.None;
    }

    public void InitializeMap()
    {
        foreach (var node in allMapNodes)
        {
            if (!node.CanRandomizeNodeType()) continue;

            var randomNodeType = SelectRandomNodeType();
            
            if (randomNodeType == MapNodeType.None) continue;

            node.nodeType = randomNodeType;
        }
    }
}
