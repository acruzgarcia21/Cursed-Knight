using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize = 10;

    [SerializeField] private DeckData startingDeck;

    [SerializeField] private CardDatabase cardDatabase;

    [SerializeField] private TMP_Text deckCount;
    
    private readonly List<RuntimeCard> _playerDeck = new();
    
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
        
        UpdateDeckCount();

        _drawPileManager.MakeDrawPile(_playerDeck);
    }

    public void InitializeRunDeck()
    {
        if (startingDeck == null) return;

        _playerDeck.Clear();

        foreach (var card in startingDeck.GetPlayerDeck())
        {
            if (card == null) continue;

            _playerDeck.Add(new RuntimeCard(card));
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
        
        _playerDeck.Add(new RuntimeCard(cardToAdd));
        
        UpdateDeckCount();
    }

    public void RemoveCardFromDeck(RuntimeCard cardToRemove)
    {
        if (cardToRemove == null) return;

        _playerDeck.Remove(cardToRemove);
        
        UpdateDeckCount();
    }

    public List<RuntimeCard> GetPlayerDeck()
    {
        return _playerDeck;
    }

    public void RestoreCardsToPlayerDeck(string cardID, bool isUpgraded)
    {
        var card = cardDatabase.GetCardByID(cardID);
        if (card == null) return;

        var runtimeCardToRestore = new RuntimeCard(card)
        {
            isUpgraded = isUpgraded
        };

        _playerDeck.Add(runtimeCardToRestore);
    }

    private void UpdateDeckCount()
    {
        deckCount.text = _playerDeck.Count.ToString();
    }
}