using System.Collections;
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

    [Header("Selection Feedback")]
    [SerializeField] private float selectionPulseDuration = 0.25f;
    [SerializeField] private float selectionPulseScale = 1.2f;

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

    public IEnumerator PlaySelectionPulse()
    {
        var originalScale = currentNodeImage.transform.localScale;
        var elapsedTime = 0f;

        while (elapsedTime < selectionPulseDuration)
        {
            elapsedTime += Time.deltaTime;
            var progress = Mathf.Clamp01(elapsedTime / selectionPulseDuration);
            var pulse = Mathf.Sin(progress * Mathf.PI);

            currentNodeImage.transform.localScale = originalScale * Mathf.Lerp(1f, selectionPulseScale, pulse);
            yield return null;
        }

        currentNodeImage.transform.localScale = originalScale;
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

    public void UpdateNodeSprite()
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
