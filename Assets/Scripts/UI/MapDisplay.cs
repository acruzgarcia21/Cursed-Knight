using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Map map;

    [SerializeField] private RectTransform connectionsContainer;
    
    [SerializeField] private GameObject connectionLinePrefab;

    [SerializeField] private float connectionBetweenNodesGap = 30f;

    private void Awake()
    {
        CreateNodeConnectionLines();
    }

    private void CreateNodeConnectionLines()
    {
        foreach (var node in map.GetAllMapNodes())
        {
            var nodeRectTransform = node.GetComponent<RectTransform>();
            
            foreach (var nextNode in node.nextNodes)
            {
                var nextNodeRectTransform = nextNode.GetComponent<RectTransform>();
                
                var startPosition = nodeRectTransform.anchoredPosition;
                var endPosition = nextNodeRectTransform.anchoredPosition;

                var midPoint = (endPosition + startPosition) / 2;
                
                var connectionBetweenNodes = Instantiate(connectionLinePrefab, connectionsContainer);

                var connectionBetweenNodesRectTransform =
                    connectionBetweenNodes.GetComponent<RectTransform>();
                
                connectionBetweenNodesRectTransform.anchoredPosition = midPoint;

                var distanceBetweenNodes = Vector2.Distance(startPosition, endPosition);

                distanceBetweenNodes -= (2 * connectionBetweenNodesGap);

                var currentSize = connectionBetweenNodesRectTransform.sizeDelta;
                
                currentSize.x = distanceBetweenNodes;

                connectionBetweenNodesRectTransform.sizeDelta = currentSize;

                var directionBetweenNodes = endPosition - startPosition;

                var angleInRadians = Mathf.Atan2(directionBetweenNodes.y, directionBetweenNodes.x);

                var angleInDegrees = angleInRadians * Mathf.Rad2Deg;

                connectionBetweenNodesRectTransform.localRotation = Quaternion.Euler(0f, 0f, angleInDegrees);
            }
        }
    }
}
