using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Map map;

    [Header("Map Containers")] 
    [SerializeField] private RectTransform actOneMapContainer;
    [SerializeField] private RectTransform actTwoMapContainer;
    [SerializeField] private RectTransform actThreeMapContainer;

    [SerializeField] private ScrollRect mapScrollRect;
    
    
    [Header("Connection Containers")]
    [SerializeField] private RectTransform actOneConnectionsContainer;
    [SerializeField] private RectTransform actTwoConnectionsContainer;
    [SerializeField] private RectTransform actThreeConnectionsContainer;
    
    [SerializeField] private RectTransform connectionsContainer;
    
    [SerializeField] private GameObject connectionLinePrefab;
    [SerializeField] private GameObject mapScreen;
    [SerializeField] private GameObject closeButton;
    
    [SerializeField] private float connectionEndGap = 60f;

    private MapMode _mapMode;

    private RunManager _runManager;
    private AudioManager _audioManager;

    private readonly List<GameObject> _connectionLines = new();

    public enum MapMode
    {
        View,
        Select
    }

    private void Awake()
    {
        _runManager = FindAnyObjectByType<RunManager>();
        _audioManager = FindAnyObjectByType<AudioManager>();
        _runManager.OnNodeChanged += HandleNodeChanged;
        
        mapScreen.SetActive(false);
        closeButton.SetActive(false);
    }

    public void SetMap(Map newMap)
    {
        if (newMap == null) return;

        RectTransform selectedContainer = _runManager.GetCurrentAct() switch
        {
            RunManager.CurrentAct.ActOne => actOneMapContainer,
            RunManager.CurrentAct.ActTwo => actTwoMapContainer,
            RunManager.CurrentAct.ActThree => actThreeMapContainer,
            _ => null
        };

        if (selectedContainer == null)
        {
            Debug.LogError("selected container is null!");
            return;
        }
        
        RectTransform selectedConnectionsContainer = _runManager.GetCurrentAct() switch
        {
            RunManager.CurrentAct.ActOne => actOneConnectionsContainer,
            RunManager.CurrentAct.ActTwo => actTwoConnectionsContainer,
            RunManager.CurrentAct.ActThree => actThreeConnectionsContainer,
            _ => null
        };

        if (selectedConnectionsContainer == null)
        {
            Debug.LogError("The current act's connection container is not assigned!");
            return;
        }

        ClearConnectionLines();
        
        if (actOneMapContainer != null) actOneMapContainer.gameObject.SetActive(false);
        if (actTwoMapContainer != null) actTwoMapContainer.gameObject.SetActive(false);
        if (actThreeMapContainer != null) actThreeMapContainer.gameObject.SetActive(false);
        
        selectedContainer.gameObject.SetActive(true);

        mapScrollRect.content = selectedContainer;
        mapScrollRect.StopMovement();

        map = newMap;
        
        connectionsContainer = selectedConnectionsContainer;
        
        CreateNodeConnectionLines();
        
        Canvas.ForceUpdateCanvases();
        mapScrollRect.horizontalNormalizedPosition = 0f;
    }

    public void HandlePostSelectNode()
    {
        if (mapScreen.activeSelf) _audioManager.PlayMapCloseSound();
        mapScreen.SetActive(false);
        closeButton.SetActive(false);
    }

    public void OpenViewMode()
    {
        _mapMode = MapMode.View;
        if (!mapScreen.activeSelf) _audioManager.PlayMapOpenSound();
        mapScreen.SetActive(true);
        closeButton.SetActive(true);
        RefreshNodeVisuals();
    }

    public void OnCloseButton()
    {
        if (mapScreen.activeSelf) _audioManager.PlayMapCloseSound();
        mapScreen.SetActive(false);
        closeButton.SetActive(false);
    }

    public void OpenSelectMode()
    {
        _mapMode = MapMode.Select;
        if (!mapScreen.activeSelf) _audioManager.PlayMapOpenSound();
        mapScreen.SetActive(true);
        RefreshNodeVisuals();
    }
    public bool IsSelectionMode()
    {
        return _mapMode == MapMode.Select;
    }

    public void RefreshNodeVisuals()
    {
        foreach (var node in map.GetAllMapNodes())
        {
            var nodeDisplay = node.GetComponent<MapNodeDisplay>();

            nodeDisplay.UpdateNodeSprite();

            if (_runManager.IsCurrentNode(node))
            {
                nodeDisplay.ApplyCurrentNodeEffects();
            }
            else if (_runManager.HasVisitedNode(node))
            {
                nodeDisplay.ApplyVisitedNodeEffects();
            }
            else
            {
                nodeDisplay.ApplyNormalNodeEffects();
            }
        }
    }

    private void CreateNodeConnectionLines()
    {
        foreach (var node in map.GetAllMapNodes())
        {
            foreach (var nextNode in node.nextNodes)
            {
                CreateConnectionLine(node, nextNode);
            }
        }
    }

    private void HandleNodeChanged(MapNode node)
    {
        RefreshNodeVisuals();
    }
    
    private void OnDestroy()
    {
        if (_runManager != null)
        {
            _runManager.OnNodeChanged -= HandleNodeChanged;
        }
    }

    private void CreateConnectionLine(MapNode startNode, MapNode endNode)
    {
        var startRect = startNode.GetComponent<RectTransform>();
        var endRect = endNode.GetComponent<RectTransform>();

        var startPosition = startRect.anchoredPosition;
        var endPosition = endRect.anchoredPosition;

        var connectionLine = Instantiate(connectionLinePrefab, connectionsContainer);
        
        _connectionLines.Add(connectionLine);
        
        var connectionRect = connectionLine.GetComponent<RectTransform>();

        var midpoint = (startPosition + endPosition) / 2;
        connectionRect.anchoredPosition = midpoint;

        var connectionLength = Vector2.Distance(startPosition, endPosition) - (connectionEndGap * 2);
        
        var connectionSize = connectionRect.sizeDelta;
        
        connectionSize.x = connectionLength;
        connectionRect.sizeDelta = connectionSize;

        var direction = endPosition - startPosition;
        
        var angleInDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        connectionRect.localRotation = Quaternion.Euler(0f, 0f, angleInDegrees);
    }

    private void ClearConnectionLines()
    {
        foreach (var connectionLine in _connectionLines)
        {
            Destroy(connectionLine);
        }
        
        _connectionLines.Clear();
    }
}