using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
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
    [SerializeField] private List<EncounterData> secondHalfBattleEncounterPool;
    [SerializeField] private List<EncounterData> eliteEncounterPool;
    [SerializeField] private List<EncounterData> bossEncounterPool;
    
    private readonly Dictionary<string, MapNode> _mapNodeDictionary = new();

    public enum MapNodeType
    {
        Battle,
        Rest,
        Elite,
        Boss,
        None
    }
    
    private void Awake()
    {
        BuildMapNodeDictionary();
    }

    private void BuildMapNodeDictionary()
    {
        Debug.Log($"MAP DICTIONARY BUILD START | Map: {gameObject.name} | Nodes: {allMapNodes.Count}");

        _mapNodeDictionary.Clear();

        foreach (var node in allMapNodes)
        {
            if (node == null || string.IsNullOrEmpty(node.GetMapNodeID()))
            {
                Debug.LogError("Map Node is not valid!");
                continue;
            }

            if (_mapNodeDictionary.ContainsKey(node.GetMapNodeID()))
            {
                Debug.LogError($"Dictionary already contains Map Node ID: {node.GetMapNodeID()}");
                continue;
            }

            _mapNodeDictionary.Add(node.GetMapNodeID(), node);

            Debug.Log($"MAP NODE ADDED | ID: {node.GetMapNodeID()} | Node: {node.name}");
        }

        Debug.Log($"MAP DICTIONARY BUILD COMPLETE | Map: {gameObject.name} | Dictionary Count: {_mapNodeDictionary.Count}");
    }

    public MapNode GetMapNodeByID(string mapNodeID)
    {
        Debug.Log($"MAP LOOKUP | Map: {gameObject.name} | Looking for: {mapNodeID} | Dictionary Count: {_mapNodeDictionary.Count}");

        if (_mapNodeDictionary.TryGetValue(mapNodeID, out var mapNode))
        {
            return mapNode;
        }

        Debug.LogError($"Dictionary does not contain the Map Node associated with ID: {mapNodeID}");
        return null;
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
                var secondHalfStartingStage = Mathf.CeilToInt(GetTotalStageCount() * 0.5f);
                var currentBattlePool = battleEncounterPool;

                if (currentNode.GetStageNumber() >= secondHalfStartingStage
                    && secondHalfBattleEncounterPool != null && secondHalfBattleEncounterPool.Count > 0)
                {
                    currentBattlePool = secondHalfBattleEncounterPool;
                }

                if (currentBattlePool == null || currentBattlePool.Count == 0) return null;

                randomNum = Random.Range(0, currentBattlePool.Count);
                randomEncounter = currentBattlePool[randomNum];
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

    public int GetTotalStageCount()
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

    public void RestoreMapNodeTypes(IReadOnlyList<MapNodeSaveData> savedMapNodesData)
    {
        foreach (var mapNodeSaveData in savedMapNodesData)
        {
            if (mapNodeSaveData == null)
            {
                Debug.LogError("Map: Unable to determine map node saved data!");
                continue;
            }
            
            var mapNodeID = mapNodeSaveData.GetNodeID();
            var mapNode = GetMapNodeByID(mapNodeID);

            if (mapNode == null)
            {
                Debug.LogError("Map: Unable to determine map node! Cannot continue restoration");
                continue;
            }

            mapNode.nodeType = mapNodeSaveData.GetNodeType();
        }
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
