using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> playerDeck = new();

    public int maxHandSize = 10;

    [SerializeField] private DeckData startingDeck;

    [SerializeField] private TMP_Text deckCount;
    
    private HandManager     _handManager;
    private DrawPileManager _drawPileManager;
    private DiscardManager  _discardManager;
    private ExhaustManager  _exhaustManager;
    

    private void Awake()
    {
        _handManager     = FindAnyObjectByType<HandManager>();
        _drawPileManager = FindAnyObjectByType<DrawPileManager>();
        _discardManager  = FindAnyObjectByType<DiscardManager>();
        _exhaustManager  = FindAnyObjectByType<ExhaustManager>();
    }

    public void BattleSetup()
    {
        _handManager.BattleSetup(maxHandSize);
        _discardManager.BattleSetup();
        _exhaustManager.BattleSetup();
        
        var runtimeCards = new List<RuntimeCard>();

        foreach (var card in playerDeck)
        {
            runtimeCards.Add(new RuntimeCard(card));
        }
        
        UpdateDeckCount();

        _drawPileManager.MakeDrawPile(runtimeCards);
    }

    public void InitializeRunDeck()
    {
        if (startingDeck == null) return;

        playerDeck.Clear();

        foreach (var card in startingDeck.GetPlayerDeck())
        {
            if (card == null) continue;

            playerDeck.Add(card);
        }
        
        UpdateDeckCount();
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

    public void RemoveCardFromDeck(Card cardToRemove)
    {
        if (cardToRemove == null) return;

        playerDeck.Remove(cardToRemove);
        
        UpdateDeckCount();
    }

    private void UpdateDeckCount()
    {
        deckCount.text = playerDeck.Count.ToString();
    }
}