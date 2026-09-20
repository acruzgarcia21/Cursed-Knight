using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public event System.Action<MapNode> OnNodeChanged;
    
    [SerializeField] private Map currentMap;
    [SerializeField] private MapDisplay mapDisplay;

    private MapNode _currentNode;
    
    private readonly HashSet<MapNode> _visitedNodes = new();

    private readonly List<MapNodeDisplay> _nodeDisplays = new();

    private BattleManager _battleManager;
    
    private void Awake()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        
        StartRun();
        SubscribeToNodeClicks();
    }

    public bool IsCurrentNode(MapNode node)
    {
        return node == _currentNode;
    }

    public bool HasVisitedNode(MapNode node)
    {
        return _visitedNodes.Contains(node);
    }

    private void EnterCurrentNode()
    {
        switch (_currentNode.nodeType)
        {
            case Map.MapNodeType.Battle:
                
                var encounter = currentMap.GenerateRandomEncounter();
                _battleManager.StartBattle(encounter);
                break;
            case Map.MapNodeType.Rest:
            case Map.MapNodeType.Elite:
            case Map.MapNodeType.Boss:
            case Map.MapNodeType.None:
                break;
        }
    }

    private void StartRun()
    {
        currentMap.InitializeMap();
        
        _currentNode = currentMap.GetStartingMapNode();
        
        EnterCurrentNode();
    }

    private void SubscribeToNodeClicks()
    {
        foreach (var node in currentMap.GetAllMapNodes())
        {
            var nodeDisplay = node.GetComponent<MapNodeDisplay>();
            nodeDisplay.OnNodeClicked += HandleNodeClicked;
            _nodeDisplays.Add(nodeDisplay);
        }
    }
    
    private void OnDestroy()
    {
        foreach (var nodeDisplay in _nodeDisplays)
        {
            if (nodeDisplay != null)
            {
                nodeDisplay.OnNodeClicked -= HandleNodeClicked;
            }
        }
    }

    private void HandleNodeClicked(MapNode clickedNode)
    {
        if (!mapDisplay.IsSelectionMode()) return;
        if (!CanMoveToNode(clickedNode)) return;
        

        MoveToNode(clickedNode);
    }
    private bool CanMoveToNode(MapNode node)
    {
        return _currentNode.nextNodes.Contains(node);
    }

    private void MoveToNode(MapNode requestedNode)
    {
        MarkCurrentNodeVisited();
        _currentNode = requestedNode;
        
        EnterCurrentNode();
        
        OnNodeChanged?.Invoke(_currentNode);
    }

    private void MarkCurrentNodeVisited()
    {
        _visitedNodes.Add(_currentNode);
    }
}