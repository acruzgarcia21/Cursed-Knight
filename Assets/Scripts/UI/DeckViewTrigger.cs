using UnityEngine;
using UnityEngine.EventSystems;

public class DeckViewTrigger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private DeckDisplay deckDisplay;
    
    private DeckManager _deckManager;

    private void Awake()
    {
        _deckManager = FindFirstObjectByType<DeckManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        deckDisplay.DisplayDeck(_deckManager.playerDeck);
    }
}
