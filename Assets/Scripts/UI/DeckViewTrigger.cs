using UnityEngine;
using UnityEngine.EventSystems;

public class DeckViewTrigger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardViewDisplay cardViewDisplay;
    
    private DeckManager _deckManager;

    private void Awake()
    {
        _deckManager = FindAnyObjectByType<DeckManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardViewDisplay.SetTitleText("Deck View");
        
        cardViewDisplay.DisplayCardDefinitions(_deckManager.playerDeck);
    }
}
