using UnityEngine;
using UnityEngine.EventSystems;

public class DrawPileViewTrigger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardViewDisplay cardViewDisplay;
    
    private DrawPileManager _drawPileManager;

    private void Awake()
    {
        _drawPileManager = FindAnyObjectByType<DrawPileManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardViewDisplay.SetTitleText("Draw Pile View");
        
        cardViewDisplay.DisplayCards(_drawPileManager.GetDrawPile());
    }
}
