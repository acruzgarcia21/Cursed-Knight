using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public event System.Action<MapNode> OnNodeChanged;
    
    [SerializeField] private Map currentMap;

    private MapNode _currentNode;
    private HashSet<MapNode> _visitedNodes = new();

    private void Awake()
    {
        StartRun();
        SubscribeToNodeClicks();
    }

    private void StartRun()
    {
        _currentNode = currentMap.GetStartingMapNode();
    }

    private void SubscribeToNodeClicks()
    {
        foreach (var node in currentMap.GetAllMapNodes())
        {
            var nodeDisplay = node.GetComponent<MapNodeDisplay>();
            nodeDisplay.OnNodeClicked += HandleNodeClicked;
        }
    }

    private void HandleNodeClicked(MapNode clickedNode)
    {
        if (CanMoveToNode(clickedNode))
        {
            MoveToNode(clickedNode);
        }
    }

    private void MarkCurrentNodeVisited()
    {
        _visitedNodes.Add(_currentNode);
    }

    private bool CanMoveToNode(MapNode node)
    {
        return _currentNode.nextNodes.Contains(node);
    }

    private void MoveToNode(MapNode requestedNode)
    {
        MarkCurrentNodeVisited();
        _currentNode = requestedNode;
        OnNodeChanged?.Invoke(_currentNode);
    }
}