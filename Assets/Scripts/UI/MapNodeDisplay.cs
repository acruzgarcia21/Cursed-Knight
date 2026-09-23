using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeDisplay : MonoBehaviour, IPointerClickHandler
{
    public event System.Action<MapNode> OnNodeClicked;
    
    [SerializeField] private MapNode currentNode;

    [SerializeField] private Image currentNodeImage;

    [SerializeField] private Color currentNodeImageColor;
    
    [SerializeField] private GameObject currentNodeHighlight;

    [SerializeField] private Sprite battleNodeSprite;
    [SerializeField] private Sprite restNodeSprite;
    [SerializeField] private Sprite eliteNodeSprite;
    [SerializeField] private Sprite bossNodeSprite;

    private Color _currentNodeImageOriginalColor;

    private void Awake()
    {
        UpdateNodeSprite();
        currentNodeHighlight.SetActive(false);

        _currentNodeImageOriginalColor = currentNodeImage.color;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnNodeClicked?.Invoke(currentNode);
    }

    public void ApplyVisitedNodeEffects()
    {
        currentNodeHighlight.SetActive(false);
        currentNodeImage.color = currentNodeImageColor;
    }
    
    public void ApplyCurrentNodeEffects()
    {
        currentNodeHighlight.SetActive(true);
        currentNodeImage.color = _currentNodeImageOriginalColor;
    }
    
    public void ApplyNormalNodeEffects()
    {
        currentNodeHighlight.SetActive(false);
        currentNodeImage.color = _currentNodeImageOriginalColor;
    }

    private void UpdateNodeSprite()
    {
        currentNodeImage.sprite = currentNode.nodeType switch
        {
            Map.MapNodeType.Battle => battleNodeSprite,
            Map.MapNodeType.Rest => restNodeSprite,
            Map.MapNodeType.Elite => eliteNodeSprite,
            Map.MapNodeType.Boss => bossNodeSprite,
            _ => currentNodeImage.sprite
        };
    }

}
