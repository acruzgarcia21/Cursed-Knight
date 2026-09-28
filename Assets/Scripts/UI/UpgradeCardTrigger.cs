using UnityEngine;

public class UpgradeCardTrigger : MonoBehaviour
{
    [SerializeField] private CardViewDisplay cardViewDisplay;
    
    private DeckManager _deckManager;
    
    private void Awake()
    {
        _deckManager = FindAnyObjectByType<DeckManager>();
    }

    public void OnCardUpgradeClicked()
    {
        cardViewDisplay.SetTitleText("Card Upgrade");
        cardViewDisplay.SetCurrentViewerToUpgradeCard();
        cardViewDisplay.DisplayCards(_deckManager.GetPlayerDeck());
    }
}
