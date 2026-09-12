using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new();

    public int maxHandSize = 10;

    private HandManager _handManager;
    private DrawPileManager _drawPileManager;
    private DiscardManager _discardManager;
    
    [SerializeField] private DeckData startingDeck;

    [SerializeField] private TMP_Text deckCount;

    private void Awake()
    {
        _drawPileManager = FindAnyObjectByType<DrawPileManager>();
        _handManager     = FindAnyObjectByType<HandManager>();
        _discardManager  = FindAnyObjectByType<DiscardManager>();
    }

    public void BattleSetup()
    {
        NewGameBattleSetup();
        
        var runtimeCards = new List<RuntimeCard>();

        foreach (var card in playerDeck)
        {
            runtimeCards.Add(new RuntimeCard(card));
        }
        
        UpdateDeckCount();

        _handManager.BattleSetup(maxHandSize);
        _drawPileManager.MakeDrawPile(runtimeCards);
    }

    public void NewGameBattleSetup()
    {
        if (startingDeck == null) return;

        playerDeck.Clear();

        foreach (var card in startingDeck.GetPlayerDeck())
        {
            if (card == null) continue;

            playerDeck.Add(card);
        }
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

    public void AddCardToDeck(Card cardToAdd)
    {
        if (cardToAdd == null) return;
        
        playerDeck.Add(cardToAdd);
    }

    public void UpdateDeckCount()
    {
        deckCount.text = playerDeck.Count.ToString();
    }
}