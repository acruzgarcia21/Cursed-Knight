using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    private DiscardPileDisplay _discardPileDisplay;
    private readonly List<Card> _discardPile = new List<Card>();
    
    private void Awake()
    {
        _discardPileDisplay = FindFirstObjectByType<DiscardPileDisplay>();
        _discardPileDisplay.UpdateDiscardCount(_discardPile);
    }

    

    public void AddToDiscardPile(Card card)
    {
        if (card == null) return;
        
        _discardPile.Add(card);
        _discardPileDisplay.UpdateDiscardCount(_discardPile);
    }

    public Card PullFromDiscardPile()
    {
        if (!IsDiscardPileEmpty())
        {
            var cardToReturn = _discardPile[^1];
            _discardPile.RemoveAt(_discardPile.Count - 1);
            _discardPileDisplay.UpdateDiscardCount(_discardPile);
            return cardToReturn;
        }
        else
        {
            return null;
        }
    }

    public bool SelectCardFromDiscardPile(Card card)
    {
        if (!IsDiscardPileEmpty() && _discardPile.Contains(card))
        {
            _discardPile.Remove(card);
            _discardPileDisplay.UpdateDiscardCount(_discardPile);
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Card> PullAllFromDiscardPile()
    {
        if (!IsDiscardPileEmpty())
        {
            var cardsToReturn = new List<Card>(_discardPile);
            _discardPile.Clear();
            _discardPileDisplay.UpdateDiscardCount(_discardPile);
            return cardsToReturn;
        }
        else
        {
            return new List<Card>();
        }
    }

    public bool IsDiscardPileEmpty()
    {
        return _discardPile.Count <= 0;
    }
    
}
