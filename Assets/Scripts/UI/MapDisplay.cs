using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Map map;
    
    [SerializeField] private RectTransform connectionsContainer;
    
    [SerializeField] private GameObject connectionLinePrefab;
    
    [SerializeField] private float connectionEndGap = 60f;


    private void Awake()
    {
        CreateNodeConnectionLines();
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