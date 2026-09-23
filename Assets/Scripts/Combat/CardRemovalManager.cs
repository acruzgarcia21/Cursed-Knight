using CursedKnight;
using UnityEngine;

public class CardRemovalManager : MonoBehaviour
{
    public event System.Action OnCardRemoved;
    
    private Card _selectedCard;

    private DeckManager _deckManager;
    
    private void Awake()
    {
        _deckManager = FindAnyObjectByType<DeckManager>();
    }

    public void SelectCard(Card cardToRemove)
    {
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

        _selectedCard = null;
        
        OnCardRemoved?.Invoke();
    }
}
