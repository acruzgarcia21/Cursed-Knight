using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    public TextMeshProUGUI drawPileCounter;

    public event System.Action OnDrawPileChanged;
    
    private List<RuntimeCard> _drawPile = new();
    
    
    private int _currentIndex;
    
    private DiscardManager _discardManager;

    private void Awake()
    {
        _discardManager = FindAnyObjectByType<DiscardManager>();
    }

    public void MakeDrawPile(List<RuntimeCard> cardsToAdd)
    {
        _drawPile.Clear();
        _drawPile.AddRange(cardsToAdd);

        Utility.Shuffle(_drawPile);

        _currentIndex = 0;
        UpdateDrawPileCount();
        
        OnDrawPileChanged?.Invoke();
    }

    public void AddToDrawPile(RuntimeCard cardToAdd)
    {
        if (cardToAdd == null) return;
        
        _drawPile.Add(cardToAdd);
        
        Utility.Shuffle(_drawPile);

        _currentIndex = 0;
        UpdateDrawPileCount();
        
        OnDrawPileChanged?.Invoke();
    }

    public RuntimeCard DrawCard()
    {
        if (_drawPile.Count == 0)
        {
            RefillDeckFromDiscard();
        }

        if (_drawPile.Count == 0) return null;
        
        var nextCard = _drawPile[_currentIndex];
        
        _drawPile.RemoveAt(_currentIndex);
        
        if (_drawPile.Count > 0)
        {
            _currentIndex %= _drawPile.Count;
        }
        else
        {
            _currentIndex = 0;
        }

        UpdateDrawPileCount();
        
        OnDrawPileChanged?.Invoke();

        return nextCard;
    }

    private void RefillDeckFromDiscard()
    {
        if (_discardManager.IsDiscardPileEmpty()) return;
        
        _drawPile = _discardManager.PullAllFromDiscardPile();
        
        Utility.Shuffle(_drawPile);
        
        _currentIndex = 0;
        UpdateDrawPileCount();
        
        OnDrawPileChanged?.Invoke();
    }

    private void UpdateDrawPileCount()
    {
        drawPileCounter.text = _drawPile.Count.ToString();
    }

    public IReadOnlyList<RuntimeCard> GetDrawPile()
    {
        return _drawPile;
    }
}