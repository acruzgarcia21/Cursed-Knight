using System.Collections.Generic;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    public event System.Action OnDiscardPileChanged;
    
    private DiscardPileDisplay _discardPileDisplay;
    private readonly List<RuntimeCard> _discardPile = new();

    private void Awake()
    {
        _discardPileDisplay = FindAnyObjectByType<DiscardPileDisplay>();
        UpdateDiscardCount();
    }

    public void BattleSetup()
    {
        _discardPile.Clear();
        UpdateDiscardCount();
        
        OnDiscardPileChanged?.Invoke();
    }

    public void AddToDiscardPile(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return;

        _discardPile.Add(runtimeCard);
        UpdateDiscardCount();
        
        OnDiscardPileChanged?.Invoke();
    }

    public RuntimeCard PullFromDiscardPile()
    {
        if (IsDiscardPileEmpty()) return null;

        var cardToReturn = _discardPile[^1];

        _discardPile.RemoveAt(_discardPile.Count - 1);
        UpdateDiscardCount();
        
        OnDiscardPileChanged?.Invoke();

        return cardToReturn;
    }

    public bool SelectCardFromDiscardPile(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return false;
        if (!_discardPile.Remove(runtimeCard)) return false;

        UpdateDiscardCount();
        
        OnDiscardPileChanged?.Invoke();
        return true;
    }

    public List<RuntimeCard> PullAllFromDiscardPile()
    {
        var cardsToReturn = new List<RuntimeCard>(_discardPile);

        _discardPile.Clear();
        UpdateDiscardCount();
        
        OnDiscardPileChanged?.Invoke();

        return cardsToReturn;
    }

    public RuntimeCard PullRandomCardFromDiscard()
    {
        if (IsDiscardPileEmpty()) return null;

        var randomCardIndex = Random.Range(0, _discardPile.Count);
        var randomCardToReturn = _discardPile[randomCardIndex];

        _discardPile.RemoveAt(randomCardIndex);
        UpdateDiscardCount();
        
        OnDiscardPileChanged?.Invoke();

        return randomCardToReturn;
    }

    public bool IsDiscardPileEmpty()
    {
        return _discardPile.Count == 0;
    }

    public IReadOnlyList<RuntimeCard> GetDiscardPile()
    {
        return _discardPile;
    }

    private void UpdateDiscardCount()
    {
        if (_discardPileDisplay == null) return;

        _discardPileDisplay.UpdateVisuals(_discardPile.Count);
    }
}