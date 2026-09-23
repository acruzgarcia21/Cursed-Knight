using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public event System.Action<MapNode> OnNodeChanged;
    
    [SerializeField] private Map currentMap;
    [SerializeField] private MapDisplay mapDisplay;
    [SerializeField] private RestScreenDisplay restScreenDisplay;

    private MapNode _currentNode;
    
    private readonly HashSet<MapNode> _visitedNodes = new();

    private readonly List<MapNodeDisplay> _nodeDisplays = new();

    private BattleManager      _battleManager;
    private DeckManager        _deckManager;
    private RestManager        _restManager;
    
    private void Awake()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _deckManager   = FindAnyObjectByType<DeckManager>();
        _restManager   = FindAnyObjectByType<RestManager>();
    }

    private void Start()
    {
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
            case Map.MapNodeType.Elite:
            case Map.MapNodeType.Boss:
                var encounter = currentMap.GenerateRandomEncounter(_currentNode);
                _battleManager.StartBattle(encounter);
                break;
            case Map.MapNodeType.Rest:
                _restManager.StartRestNode();
                break;
            case Map.MapNodeType.None:
                break;
        }
    }

    private void StartRun()
    {
        _deckManager.InitializeRunDeck();
        
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
        
        mapDisplay.HandlePostSelectNode();
        
        EnterCurrentNode();
        
        OnNodeChanged?.Invoke(_currentNode);
    }

    private void MarkCurrentNodeVisited()
    {
        _visitedNodes.Add(_currentNode);
    }

    private void OnEnable()
    {
        _restManager.OnRestCompleted += OnRestCompleted;
    }

    private void OnDisable()
    {
        _restManager.OnRestCompleted -= OnRestCompleted;
    }

    private void OnRestCompleted()
    {
        restScreenDisplay.HideRestScreen();
        mapDisplay.OpenSelectMode();
    }
}