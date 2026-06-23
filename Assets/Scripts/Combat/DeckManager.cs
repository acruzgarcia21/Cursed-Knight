using System;
using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> allCards = new List<Card>();

    public int startingHandSize = 5;

    public int maxHandSize = 10;
    public int currentHandSize;
    
    private HandManager _handManager;
    private DrawPileManager _drawPileManager;
    private bool _startBattleRun = true;

    private void Start()
    {
        // Load all card assets from the Resources folder
        var cards = Resources.LoadAll<Card>("Cards");

        // Add the loaded cards to the allCards list
        allCards.AddRange(cards);
    }

    private void Awake()
    {
        if (_drawPileManager == null)
        {
            _drawPileManager = FindFirstObjectByType<DrawPileManager>();
        }

        if (_handManager == null)
        {
            _handManager = FindFirstObjectByType<HandManager>();
        }
    }

    private void Update()
    {
        if (_startBattleRun)
        {
            BattleSetup();
        }
    }

    public void BattleSetup()
    {
        _handManager.BattleSetup(maxHandSize);
        _drawPileManager.MakeDrawPile(allCards);
        //_drawPileManager.BattleSetup(startingHandSize, maxHandSize);
        _startBattleRun = false;
    }
}
