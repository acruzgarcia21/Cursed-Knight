using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardPileViewTrigger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardViewDisplay cardViewDisplay;
    
    private DiscardManager _discardManager;
    
    private void Awake()
    {
        _discardManager = FindAnyObjectByType<DiscardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardViewDisplay.SetTitleText("Discard Pile View");
        cardViewDisplay.SetCurrentViewerToDiscardPile();
        cardViewDisplay.DisplayCards(_discardManager.GetDiscardPile());
    }
}
