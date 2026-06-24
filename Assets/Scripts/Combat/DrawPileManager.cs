using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    public List<Card> drawPile = new List<Card>();
    
    public TextMeshProUGUI drawPileCounter;
    
    private int _currentIndex = 0;
    
    private DiscardManager _discardManager;

    public void MakeDrawPile(List<Card> cardsToAdd)
    {
        drawPile.Clear();
        drawPile.AddRange(cardsToAdd);

        Utility.Shuffle(drawPile);
        UpdateDrawPileCount();
    }

    /*public void BattleSetup(int numberOfCardsToDraw, int setMaxHandSize)
    {
        maxHandSize = setMaxHandSize;
        for (var i = 0 ; i < numberOfCardsToDraw; i++)
        {
            DrawCard();
        }
    }*/

    public Card DrawCard()
    {
        if (drawPile.Count == 0)
        {
            RefillDeckFromDiscard();
        }

        if (drawPile.Count == 0) return null;
        
        var nextCard = drawPile[_currentIndex];
        
        drawPile.RemoveAt(_currentIndex);
        
        UpdateDrawPileCount();
        if (drawPile.Count > 0) _currentIndex %= drawPile.Count;
        return nextCard;
    }

    private void RefillDeckFromDiscard()
    {
        if (_discardManager == null)
        {
            _discardManager = FindFirstObjectByType<DiscardManager>();
        }

        if (_discardManager == null || _discardManager.discardCardsCount <= 0) return;
        
        drawPile = _discardManager.PullAllFromDiscardPile();
        Utility.Shuffle(drawPile);
        _currentIndex = 0;
    }

    private void UpdateDrawPileCount()
    {
        drawPileCounter.text = drawPile.Count.ToString();
    }
}
