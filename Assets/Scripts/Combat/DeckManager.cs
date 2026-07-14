using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new();

    public int maxHandSize = 10;

    private HandManager _handManager;
    private DrawPileManager _drawPileManager;

    [SerializeField] private DeckData startingDeck;

    private void Awake()
    {
        _drawPileManager = FindFirstObjectByType<DrawPileManager>();
        _handManager     = FindFirstObjectByType<HandManager>();
    }

    public void BattleSetup()
    {
        if (startingDeck == null) return;

        playerDeck.Clear();

        foreach (var card in startingDeck.GetPlayerDeck())
        {
            if (card == null) continue;

            playerDeck.Add(card);
        }

        var runtimeCards = new List<RuntimeCard>();

        foreach (var card in playerDeck)
        {
            runtimeCards.Add(new RuntimeCard(card));
        }

        _handManager.BattleSetup(maxHandSize);
        _drawPileManager.MakeDrawPile(runtimeCards);
    }
}