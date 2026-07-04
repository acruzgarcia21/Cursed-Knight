using System.Collections.Generic;
using UnityEngine;
using CursedKnight;
using UnityEngine.XR;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    
    public Transform handTransform;
    
    public int maxCardsInHand;
    
    private readonly List<GameObject> _cardsInHand = new List<GameObject>();
    
    private DiscardManager _discardManager;
    private DrawPileManager _drawPileManager;
    private HandDisplay _handDisplay;

    private void Awake()
    {
        _discardManager  = FindFirstObjectByType<DiscardManager>();
        _drawPileManager = FindFirstObjectByType<DrawPileManager>();
        _handDisplay     = FindFirstObjectByType<HandDisplay>();
    }
    
    public void BattleSetup(int setMaxHandSize)
    {
        maxCardsInHand = setMaxHandSize;
    }

    public void AddCardToHand(Card cardData)
    {
        if (_cardsInHand.Count >= maxCardsInHand) return;
        
        // Instantiate card
        // 1. GameObject
        // 2. GameObject Position
        // 3. GameObject Rotation (Quaternion.identity = no rotation)
        // 4. Transform Parent
        var newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        _cardsInHand.Add(newCard);
        
        // Set the card data of the instantiated card
        newCard.GetComponent<CardDisplay>().cardData = cardData;

        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }
    
    public void PrepareHandForTurn(int targetHandSize)
    {
        var numCardsInHand = _cardsInHand.Count;
        var numCardsToDraw = targetHandSize - numCardsInHand;

        if (numCardsToDraw <= 0) return;
        
        DrawCards(numCardsToDraw);
    }

    public void DrawCards(int numCardsToDraw)
    {
        for (var i = 0; i < numCardsToDraw; i++)
        {
            var cardToDraw = _drawPileManager.DrawCard();
            
            if (cardToDraw == null) return;
            AddCardToHand(cardToDraw);
        }
    }

    public void DiscardHand()
    {
        foreach (var card in _cardsInHand)
        {
            var cardData = card.GetComponent<CardDisplay>().cardData;
            _discardManager.AddToDiscardPile(cardData);
            Destroy(card.gameObject);
        }
        _cardsInHand.Clear();
        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }

    public void DiscardRandomCards(int numCardsToDiscard)
    {
        for (var i = 0; i < numCardsToDiscard; i++)
        {
            if (_cardsInHand.Count <= 0) break;
            
            var randomCardIndex = Random.Range(0, _cardsInHand.Count);
            
            var cardToDiscard = _cardsInHand[randomCardIndex];
            var cardData = cardToDiscard.GetComponent<CardDisplay>().cardData;
            Debug.Log($"Discarded: {cardData.cardName}");
            
            _discardManager.AddToDiscardPile(cardData);
            _cardsInHand.Remove(cardToDiscard);
            
            Destroy(cardToDiscard);
        }
        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }

    public void RemoveCardFromHand(GameObject cardToRemove)
    {
        _cardsInHand.Remove(cardToRemove);
        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }

    public bool IsHandFull()
    {
        return _cardsInHand.Count >= maxCardsInHand;
    }
}
