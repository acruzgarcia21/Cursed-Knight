using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public event System.Action<MapNode> OnNodeChanged;
    
    [Header("Displays")]
    [SerializeField] private Map currentMap;
    [SerializeField] private MapDisplay mapDisplay;
    [SerializeField] private RestScreenDisplay restScreenDisplay;
    [SerializeField] private BattleRewardDisplay battleRewardDisplay;
    [SerializeField] private RunCompleteScreenDisplay runCompleteScreenDisplay;

    private MapNode _currentNode;
    
    private readonly HashSet<MapNode> _visitedNodes = new();

    private readonly List<MapNodeDisplay> _nodeDisplays = new();

    private BattleManager _battleManager;
    private DeckManager   _deckManager;
    private RestManager   _restManager;
    private RelicManager  _relicManager;
    private RewardManager _rewardManager;
    private SaveManager _saveManager;

    private Player _player;
    
    public enum CurrentAct
    {
        ActOne,
        ActTwo,
        ActThree
    }

    private CurrentAct _currentAct;
    
    private void Awake()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _deckManager   = FindAnyObjectByType<DeckManager>();
        _restManager   = FindAnyObjectByType<RestManager>();
        _relicManager  = FindAnyObjectByType<RelicManager>();
        _rewardManager = FindAnyObjectByType<RewardManager>();
        _saveManager   = FindAnyObjectByType<SaveManager>();
        
        _player = FindAnyObjectByType<Player>();
    }

    private void Start()
    {
        // StartRun();
        SubscribeToNodeClicks();
    }
    
    public void FinishRunRestoration()
    {
        mapDisplay.OpenSelectMode();
    }

    public void RestoreRun(CurrentAct currentAct, string currentNodeID, IReadOnlyList<string> visitedNodeIDs)
    {
        _currentAct = currentAct;

        var currentNode = currentMap.GetMapNodeByID(currentNodeID);

        if (currentNode == null)
        {
            Debug.LogError($"RunManager: {currentNode} is null, cannot finish restoring run!");
            return;
        }

        _currentNode = currentNode;
        
        _visitedNodes.Clear();

        foreach (var visitedNodeID in visitedNodeIDs)
        {
            if (string.IsNullOrEmpty(visitedNodeID))
            {
                Debug.LogError($"Run Manager: {visitedNodeID} is null or empty!");
                continue;
            }

            var visitedNode = currentMap.GetMapNodeByID(visitedNodeID);
            
            if (visitedNode == null)
            {
                Debug.LogError($"RunManager: {visitedNode} is null, cannot finish restoring run!");
                continue;
            }
            
            _visitedNodes.Add(visitedNode);
        }
    }

    public CurrentAct GetCurrentAct()
    {
        return _currentAct;
    }

    public Map GetCurrentMap()
    {
        return currentMap;
    }

    public string GetCurrentNodeID()
    {
        return _currentNode.GetMapNodeID();
    }

    public bool IsCurrentNode(MapNode node)
    {
        return node == _currentNode;
    }

    public bool HasVisitedNode(MapNode node)
    {
        return _visitedNodes.Contains(node);
    }

    public HashSet<MapNode> GetVisitedNodes()
    {
        return _visitedNodes;
    }

    private void EnterCurrentNode()
    {
        _saveManager.DisableRunSaving();
        
        var finalStageNum = currentMap.GetTotalStageCount();
        var isFinalStage = (_currentNode.GetStageNumber() - 1) == finalStageNum;
        
        switch (_currentNode.nodeType)
        {
            case Map.MapNodeType.Battle:
            case Map.MapNodeType.Elite:
            case Map.MapNodeType.Boss:
                var encounter = currentMap.GenerateRandomEncounter(_currentNode);
                _battleManager.StartBattle(encounter, _currentNode.nodeType, isFinalStage);
                break;
            case Map.MapNodeType.Rest:
                _restManager.StartRestNode();
                break;
            case Map.MapNodeType.None:
                break;
        }
    }

    public void StartNewRun()
    {
        _saveManager.DisableRunSaving();
        
        _currentAct = CurrentAct.ActOne;
        
        _deckManager.InitializeRunDeck();
        
        _relicManager.RunSetup(_currentAct);
        
        _rewardManager.ActSetup();
        
        _player.StartRun();
        
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
        battleRewardDisplay.OnRewardSelectionCompleted += OnRewardSelectionCompleted;
    }

    private void OnDisable()
    {
        _restManager.OnRestCompleted -= OnRestCompleted;
        battleRewardDisplay.OnRewardSelectionCompleted -= OnRewardSelectionCompleted;
    }

    private void OnRestCompleted()
    {
        restScreenDisplay.HideRestScreen();
        mapDisplay.OpenSelectMode();
        
        _saveManager.SaveCheckpoint();
    }

    private void OnRewardSelectionCompleted()
    {
        Debug.Log(_currentNode.nodeType);
        
        if (_currentNode.nodeType == Map.MapNodeType.Boss)
        {
            MoveToNextAct();
        }
        else
        {
            mapDisplay.OpenSelectMode();
            _saveManager.SaveCheckpoint();
        }
    }

    private void MoveToNextAct()
    {
        switch (_currentAct)
        {
            case CurrentAct.ActOne:
                _currentAct = CurrentAct.ActTwo;
                _relicManager.LoadRelicPool(_currentAct);
                _rewardManager.ActSetup();
                break;
            
            case CurrentAct.ActTwo:
                _currentAct = CurrentAct.ActThree;
                _relicManager.LoadRelicPool(_currentAct);
                _rewardManager.ActSetup();
                break;
            case CurrentAct.ActThree:
                runCompleteScreenDisplay.DisplayRunCompleteScreen();
                break;
        }
    }
}