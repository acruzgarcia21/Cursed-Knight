using System.Collections.Generic;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    private DiscardPileDisplay _discardPileDisplay;
    private readonly List<RuntimeCard> _discardPile = new();

    private void Awake()
    {
        _discardPileDisplay = FindFirstObjectByType<DiscardPileDisplay>();
        UpdateDiscardCount();
    }

    public void AddToDiscardPile(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return;

        _discardPile.Add(runtimeCard);
        UpdateDiscardCount();
    }

    public RuntimeCard PullFromDiscardPile()
    {
        if (IsDiscardPileEmpty()) return null;

        var cardToReturn = _discardPile[^1];

        _discardPile.RemoveAt(_discardPile.Count - 1);
        UpdateDiscardCount();

        return cardToReturn;
    }

    public bool SelectCardFromDiscardPile(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return false;
        if (!_discardPile.Remove(runtimeCard)) return false;

        UpdateDiscardCount();
        return true;
    }

    public List<RuntimeCard> PullAllFromDiscardPile()
    {
        var cardsToReturn = new List<RuntimeCard>(_discardPile);

        _discardPile.Clear();
        UpdateDiscardCount();

        return cardsToReturn;
    }

    public RuntimeCard PullRandomCardFromDiscard()
    {
        if (IsDiscardPileEmpty()) return null;

        var randomCardIndex = Random.Range(0, _discardPile.Count);
        var randomCardToReturn = _discardPile[randomCardIndex];

        _discardPile.RemoveAt(randomCardIndex);
        UpdateDiscardCount();

        return randomCardToReturn;
    }

    public bool IsDiscardPileEmpty()
    {
        return _discardPile.Count == 0;
    }

    private void UpdateDiscardCount()
    {
        if (_discardPileDisplay == null) return;

        _discardPileDisplay.UpdateDiscardCount(_discardPile.Count);
    }
}