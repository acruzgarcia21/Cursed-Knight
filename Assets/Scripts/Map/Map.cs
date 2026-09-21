using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    [Header("Map Node Data")] 
    [SerializeField] private List<MapNode> allMapNodes;
    
    [SerializeField] private MapNode startingMapNode;
    
    // private MapNode _bossNode;
    
    [Space(10)] [Header("Random Node Generation Data")]
    [SerializeField] private List<RandomNodeType> randomNodeTypes;
    
    [Space(10)] [Header("Restrictions")]
    [SerializeField] private int maxEliteNodeCount = 5;

    [Space(10)] [Header("Pool Data")]
    [SerializeField] private List<EncounterData> battleEncounterPool;
    [SerializeField] private List<EncounterData> eliteEncounterPool;
    [SerializeField] private List<EncounterData> bossEncounterPool;



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

    private MapNodeType SelectRandomNodeType(int eliteNodeCount, MapNode currentNode, int protectedStageCount)
    {
        List<RandomNodeType> eligibleRandomNodeTypes = new();
        
        var currentNodePreviousNodes = DetermineNodesLeadingToGivenNode(currentNode);

        var hasPreviousRest = false;
        var hasPreviousElite = false;

        foreach (var previousNode in currentNodePreviousNodes)
        {
            switch (previousNode.nodeType)
            {
                case MapNodeType.Rest:
                    hasPreviousRest = true;
                    break;
                case MapNodeType.Elite:
                    hasPreviousElite = true;
                    break;
            }
        }
        
        
        // Calculate the total weight of the pool
        var totalWeight = 0;

        foreach (var randomNodeType in randomNodeTypes)
        {
            if (randomNodeType.GetSelectionWeight() <= 0) continue;
            
            if (eliteNodeCount >= maxEliteNodeCount 
                && randomNodeType.GetNodeType() == MapNodeType.Elite) continue;
            
            if (currentNode.GetStageNumber() < protectedStageCount 
                && randomNodeType.GetNodeType() == MapNodeType.Elite) continue;
            
            if (currentNode.GetStageNumber() < protectedStageCount 
                && randomNodeType.GetNodeType() == MapNodeType.Rest) continue;
            
            if (hasPreviousElite && randomNodeType.GetNodeType() == MapNodeType.Elite) continue;
            if (hasPreviousRest && randomNodeType.GetNodeType() == MapNodeType.Rest) continue;
            
            eligibleRandomNodeTypes.Add(randomNodeType);
            
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
        foreach (var randomNodeType in eligibleRandomNodeTypes)
        {
            runningWeight += randomNodeType.GetSelectionWeight();

            if (roll >= runningWeight) continue;
            
            return randomNodeType.GetNodeType();
        }

        return MapNodeType.None;
    }

    public void InitializeMap()
    {
        var eliteNodeCount = 0;

        var totalStageCount = GetTotalStageCount();
        var protectedStageCount = Mathf.CeilToInt(totalStageCount * 0.25f);

        var copyOfAllMapNodes = new List<MapNode>(allMapNodes);
        
        copyOfAllMapNodes.Sort((a, b) 
            => a.GetStageNumber().CompareTo(b.GetStageNumber()));
        
        foreach (var node in copyOfAllMapNodes)
        {
            if (!node.CanRandomizeNodeType()) continue;
            
            var randomNodeType = SelectRandomNodeType(eliteNodeCount, node, protectedStageCount);

            if (randomNodeType == MapNodeType.Elite) eliteNodeCount++;
            
            if (randomNodeType == MapNodeType.None) continue;

            node.nodeType = randomNodeType;
        }
    }

    public EncounterData GenerateRandomEncounter(MapNode currentNode)
    {
        EncounterData randomEncounter = null;
        var randomNum = 0;
        
        switch (currentNode.nodeType)
        {
            case MapNodeType.Battle:
                if (battleEncounterPool == null || battleEncounterPool.Count == 0) return null;

                randomNum = Random.Range(0, battleEncounterPool.Count);
                randomEncounter = battleEncounterPool[randomNum];
                break;

            case MapNodeType.Elite:
                if (eliteEncounterPool == null || eliteEncounterPool.Count == 0) return null;

                randomNum = Random.Range(0, eliteEncounterPool.Count);
                randomEncounter = eliteEncounterPool[randomNum];
                break;

            case MapNodeType.Boss:
                if (bossEncounterPool == null || bossEncounterPool.Count == 0) return null;

                randomNum = Random.Range(0, bossEncounterPool.Count);
                randomEncounter = bossEncounterPool[randomNum];
                break;
        }

        return randomEncounter;
    }

    private int GetTotalStageCount()
    {
        var totalStageNum = 0;
        
        foreach (var node in GetAllMapNodes())
        {
            if (node.GetStageNumber() > totalStageNum)
            {
                totalStageNum = node.GetStageNumber();
            }
        }

        return totalStageNum + 1;
    }

    private List<MapNode> DetermineNodesLeadingToGivenNode(MapNode givenNode)
    {
        List<MapNode> nodesLeadingToGivenNode = new();
            
        foreach (var node in GetAllMapNodes())
        {
            if (node.nextNodes.Contains(givenNode))
            {
                nodesLeadingToGivenNode.Add(node);
            }
        }

        return nodesLeadingToGivenNode;
    }
}
