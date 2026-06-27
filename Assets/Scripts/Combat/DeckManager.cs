using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new List<Card>();
    
    public int maxHandSize = 10;
    
    private HandManager _handManager;
    private DrawPileManager _drawPileManager;

    [SerializeField] private DeckData startingDeck;

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
         
    public void BattleSetup()
    {
        if (startingDeck == null) return;
        
        playerDeck.Clear();

        foreach (Card card in startingDeck.GetPlayerDeck())
        {
            playerDeck.Add(card);
        }
        
        _handManager.BattleSetup(maxHandSize);
        _drawPileManager.MakeDrawPile(playerDeck);
    }
}
