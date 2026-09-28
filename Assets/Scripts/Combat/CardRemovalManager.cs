using UnityEngine;

public class CardRemovalManager : MonoBehaviour
{
    public event System.Action OnCardRemoved;
    
    private RuntimeCard _selectedCard;

    private DeckManager _deckManager;
    
    private void Awake()
    {
        _deckManager = FindAnyObjectByType<DeckManager>();
    }

    public void SelectCard(RuntimeCard cardToRemove)
    {
        ClearSelectedCard();
        _selectedCard = cardToRemove;
        Debug.Log("Selected Card: " + _selectedCard + ". Card To Remove: " + cardToRemove + ".");
    }
    
    public void ClearSelectedCard()
    {
        _selectedCard = null;
    }

    public void ResolveCardRemovalConfirmation()
    {
        if (_selectedCard == null) return;
        
        _deckManager.RemoveCardFromDeck(_selectedCard);

        ClearSelectedCard();
        
        OnCardRemoved?.Invoke();
    }
}
