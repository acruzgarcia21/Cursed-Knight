using System.Collections.Generic;
using UnityEngine;
using CursedKnight;

public class HandManager : MonoBehaviour
{
    // Assign Card Prefab In Inspector
    public GameObject cardPrefab;
    // Root of the Hand Position
    public Transform handTransform;
    // Determines how much our hand will be spread out
    public float fanSpread = 7.5f;
    // Hold a list of card objects in player's hand
    public List<GameObject> cardsInHand = new List<GameObject>();
    public int maxCardsInHand;
    
    public float cardSpacing = 100f;
    public float verticalSpacing = 100f;
    
    private DiscardManager _discardManager;

    private void Awake()
    {
        _discardManager = FindFirstObjectByType<DiscardManager>();
    }
    
    public void BattleSetup(int setMaxHandSize)
    {
        maxCardsInHand = setMaxHandSize;
    }

    public void AddCardToHand(Card cardData)
    {
        if (cardsInHand.Count >= maxCardsInHand)
        {
            Debug.Log("You already have maximum amount of cards!: " + cardsInHand.Count);
            return;
        }
        // Instantiate card
        // 1. GameObject
        // 2. GameObject Position
        // 3. GameObject Rotation (Quaternion.identity = no rotation)
        // 4. Transform Parent
        var newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);
        
        // Set the card data of the instantiated card
        newCard.GetComponent<CardDisplay>().cardData = cardData;

        UpdateHandVisuals();
    }

    public void DiscardHand()
    {
        foreach (var card in cardsInHand)
        {
            var cardData = card.GetComponent<CardDisplay>().cardData;
            _discardManager.AddToDiscardPile(cardData);
            Destroy(card.gameObject);
        }
        cardsInHand.Clear();
        UpdateHandVisuals();
    }

    private void UpdateHandVisuals()
    {
        var cardCount = cardsInHand.Count;

        // Error handling for 1 card in hand
        if (cardCount == 1)
        {
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }
        
        for (var i = 0; i < cardCount; i++)
        {
            // Goes through every single card and goes through each rotation
            var rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            // Helps cards visually offset in both vertical and horizontal so that cards are not stacked on each other
            var horizontalOffset = (cardSpacing * (i - (cardCount - 1) / 2f));
            // Normalize card position between -1 and 1
            var normalizedPosition = (2f * i / (cardCount - 1f) - 1f);
            var verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);
            
            // Set card positions
            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
        }
    }
}
