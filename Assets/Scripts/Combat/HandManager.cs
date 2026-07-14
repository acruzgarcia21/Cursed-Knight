using System.Collections.Generic;
using UnityEngine;
using CursedKnight;
using UnityEngine.XR;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handTransform;
    
    public int maxCardsInHand;
    
    private readonly List<GameObject> _cardsInHand = new();
    
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

    public void AddCardToHand(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return;
        if (_cardsInHand.Count >= maxCardsInHand) return;
        
        // Instantiate card
        // 1. GameObject
        // 2. GameObject Position
        // 3. GameObject Rotation (Quaternion.identity = no rotation)
        // 4. Transform Parent
        var newCard = Instantiate(
            cardPrefab, 
            handTransform.position, 
            Quaternion.identity, 
            handTransform);
        
        var cardDisplay = newCard.GetComponent<CardDisplay>();

        if (cardDisplay == null)
        {
            Destroy(newCard);
            Debug.LogError("Card prefab is missing CardDisplay.");
            return;
        }

        cardDisplay.runtimeCard = runtimeCard;

        _cardsInHand.Add(newCard);
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
            if (IsHandFull()) break;

            var cardToDraw = _drawPileManager.DrawCard();

            if (cardToDraw == null) break;

            AddCardToHand(cardToDraw);
        }
    }

    public void DiscardHand()
    {
        foreach (var cardObject in _cardsInHand)
        {
            var cardDisplay = cardObject.GetComponent<CardDisplay>();

            if (cardDisplay != null && cardDisplay.runtimeCard != null)
            {
                _discardManager.AddToDiscardPile(cardDisplay.runtimeCard);
            }

            Destroy(cardObject);
        }

        _cardsInHand.Clear();
        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }

    public void DiscardRandomCards(int numCardsToDiscard)
    {
        for (var i = 0; i < numCardsToDiscard; i++)
        {
            if (_cardsInHand.Count == 0) break;

            var randomCardIndex = Random.Range(0, _cardsInHand.Count);
            var cardObject = _cardsInHand[randomCardIndex];
            var cardDisplay = cardObject.GetComponent<CardDisplay>();

            if (cardDisplay != null && cardDisplay.runtimeCard != null)
            {
                _discardManager.AddToDiscardPile(cardDisplay.runtimeCard);
            }

            _cardsInHand.RemoveAt(randomCardIndex);
            Destroy(cardObject);
        }

        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }

    public void RemoveCardFromHand(GameObject cardToRemove)
    {
        if (cardToRemove == null) return;

        _cardsInHand.Remove(cardToRemove);
        _handDisplay.UpdateHandVisuals(_cardsInHand);
    }

    public bool IsHandFull()
    {
        return _cardsInHand.Count >= maxCardsInHand;
    }
}
