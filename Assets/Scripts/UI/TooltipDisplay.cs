using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipDisplay : MonoBehaviour
{
    [SerializeField] private GameObject toolTipPanel;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private float tooltipOffsetX = 10f;
    [SerializeField] private float backgroundPadding = 10f;
    [SerializeField] private float bottomPadding = 5f;
    [SerializeField] private float borderPadding = 10f;

    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private RectTransform borderTransform;
    [SerializeField] private RectTransform mainBackgroundTransform;
    
    private RectTransform _tooltipPanelRectTransform;

    private void Awake()
    {
        toolTipPanel.SetActive(false);

        _tooltipPanelRectTransform = toolTipPanel.GetComponent<RectTransform>();
    }

    public void ShowTooltip(TooltipData tooltipData, RectTransform sourcePoint)
    {
        toolTipPanel.SetActive(true);
        
        titleText.text       = tooltipData.GetTooltipTitle();
        descriptionText.text = tooltipData.GetTooltipDescription();
        
        titleText.ForceMeshUpdate();
        descriptionText.ForceMeshUpdate();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);

        var sourcePointInCanvas = canvasTransform.InverseTransformPoint(sourcePoint.position);

        var sourceHalfWidth  = sourcePoint.rect.width / 2f;
        var tooltipHalfWidth = _tooltipPanelRectTransform.rect.width / 2f;
        var backgroundWidth  = contentTransform.rect.width + backgroundPadding;
        var backgroundHeight = contentTransform.rect.height + backgroundPadding + bottomPadding;

        var backgroundNewSize = new Vector2(backgroundWidth, backgroundHeight);
        var borderNewSize = new Vector2(backgroundWidth + borderPadding, backgroundHeight + borderPadding);

        mainBackgroundTransform.sizeDelta = backgroundNewSize;
        borderTransform.sizeDelta = borderNewSize;

        var backgroundOffsetY = -bottomPadding / 2f;

        mainBackgroundTransform.anchoredPosition = new Vector2(
            mainBackgroundTransform.anchoredPosition.x,
            backgroundOffsetY
        );

        borderTransform.anchoredPosition = new Vector2(
            borderTransform.anchoredPosition.x,
            backgroundOffsetY
        );
        
        if (sourcePointInCanvas.x < 0)
        {
            var tooltipX = 
                sourcePointInCanvas.x + sourceHalfWidth + tooltipHalfWidth + tooltipOffsetX;
            
            toolTipPanel.transform.localPosition = new Vector3(tooltipX, sourcePointInCanvas.y, 0);
        }
        else
        {
            var tooltipX = 
                sourcePointInCanvas.x - sourceHalfWidth - tooltipHalfWidth - tooltipOffsetX;
            
            toolTipPanel.transform.localPosition = new Vector3(tooltipX, sourcePointInCanvas.y, 0);
        }
    }

    public void HideTooltip()
    {
        toolTipPanel.SetActive(false);
    }
}
