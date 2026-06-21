using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    public List<Card> drawPile = new List<Card>();
    
    public int maxHandSize;
    public int currentHandSize;
    
    public TextMeshProUGUI drawPileCounter;
    
    private int _currentIndex = 0;

    private HandManager _handManager;
    
    private DiscardManager _discardManager;
    private void Start()
    {
        _handManager = FindFirstObjectByType<HandManager>();
    }

    private void Update()
    {
        if (_handManager != null)
        {
            currentHandSize = _handManager.cardsInHand.Count;
        }
    }

    public void MakeDrawPile(List<Card> cardsToAdd)
    {
        drawPile.AddRange(cardsToAdd);
        Utility.Shuffle(drawPile);
        UpdateDrawPileCount();
    }

    public void BattleSetup(int numberOfCardsToDraw, int setMaxHandSize)
    {
        maxHandSize = setMaxHandSize;
        for (var i = 0 ; i < numberOfCardsToDraw; i++)
        {
            DrawCard(_handManager);
        }
    }

    public void DrawCard(HandManager handManager)
    {
        if (drawPile.Count == 0)
        {
            RefillDeckFromDiscard();
        }
        
        if (drawPile.Count == 0) return;
        if (currentHandSize >= maxHandSize) return;
        
        var nextCard = drawPile[_currentIndex];
        handManager.AddCardToHand(nextCard);
        drawPile.RemoveAt(_currentIndex);
        UpdateDrawPileCount();
        if (drawPile.Count > 0) _currentIndex %= drawPile.Count;
    }

    public void DrawCards(int numCardsToDraw)
    {
        for (var i = 0; i < numCardsToDraw; i++)
        {
            DrawCard(_handManager);
        }
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
