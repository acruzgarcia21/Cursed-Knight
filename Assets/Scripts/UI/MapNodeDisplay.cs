using UnityEngine;
using UnityEngine.UI;

public class MapNodeDisplay : MonoBehaviour
{
    [SerializeField] private MapNode currentNode;

    [SerializeField] private Image currentNodeImage;

    [SerializeField] private Sprite battleNodeSprite;
    [SerializeField] private Sprite restNodeSprite;
    [SerializeField] private Sprite eliteNodeSprite;
    [SerializeField] private Sprite bossNodeSprite;

    private void Awake()
    {
        UpdateNodeSprite();
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
