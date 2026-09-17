using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Map map;
    
    [SerializeField] private RectTransform connectionsContainer;
    
    [SerializeField] private GameObject connectionLinePrefab;
    [SerializeField] private GameObject mapScreen;
    [SerializeField] private GameObject closeButton;
    
    [SerializeField] private float connectionEndGap = 60f;

    private MapMode _mapMode;

    public enum MapMode
    {
        View,
        Select
    }

    private void Awake()
    {
        mapScreen.SetActive(false);
        closeButton.SetActive(false);
        CreateNodeConnectionLines();
    }

    public void OpenViewMode()
    {
        _mapMode = MapMode.View;
        mapScreen.SetActive(true);
        closeButton.SetActive(true);
    }

    public void OnCloseButton()
    {
        mapScreen.SetActive(false);
        closeButton.SetActive(false);
    }

    public void OpenSelectMode()
    {
        _mapMode = MapMode.Select;
        mapScreen.SetActive(true);
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

    public bool IsSelectionMode()
    {
        return _mapMode == MapMode.Select;
    }

    private void CreateConnectionLine(MapNode startNode, MapNode endNode)
    {
        var startRect = startNode.GetComponent<RectTransform>();
        var endRect = endNode.GetComponent<RectTransform>();

        var startPosition = startRect.anchoredPosition;
        var endPosition = endRect.anchoredPosition;

        var connectionLine = Instantiate(connectionLinePrefab, connectionsContainer);
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
}