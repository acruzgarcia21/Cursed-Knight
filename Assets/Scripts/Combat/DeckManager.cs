using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new();

    public int maxHandSize = 10;

    private HandManager _handManager;
    private DrawPileManager _drawPileManager;
    private DiscardManager _discardManager;
    

    [SerializeField] private DeckData startingDeck;

    private void Awake()
    {
        _drawPileManager = FindFirstObjectByType<DrawPileManager>();
        _handManager     = FindFirstObjectByType<HandManager>();
        _discardManager  = FindFirstObjectByType<DiscardManager>();
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

    public void CreateCardDuringCombat(Card cardData, Card.CreatedCardDestination destination)
    {
        if (cardData == null) return;

        var createdCard = new RuntimeCard(cardData, true);

        switch (destination)
        {
            case Card.CreatedCardDestination.Hand:
                _handManager.AddCardToHand(createdCard);
                break;
            case Card.CreatedCardDestination.DrawPile:
                _drawPileManager.AddToDrawPile(createdCard);
                break;
            case Card.CreatedCardDestination.DiscardPile:
                _discardManager.AddToDiscardPile(createdCard);
                break;
        }
    }
}