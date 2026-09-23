using UnityEngine;

public class CardRemovalTrigger : MonoBehaviour
{
    [SerializeField] private CardViewDisplay cardViewDisplay;
    
    private DeckManager _deckManager;
    
    private void Awake()
    {
        _deckManager = FindAnyObjectByType<DeckManager>();
    }

    public void OnCardRemovalClicked()
    {
        cardViewDisplay.SetTitleText("Card Removal");
        cardViewDisplay.SetCurrentViewerToCardRemoval();
        cardViewDisplay.DisplayCardDefinitions(_deckManager.playerDeck);
    }
}
