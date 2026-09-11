using System;
using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DeckDisplay : MonoBehaviour
{
    [SerializeField] private GameObject deckViewScreen;
    [SerializeField] private GameObject cardContainer;
    [SerializeField] private GameObject cardPrefab;

    private readonly List<GameObject> _cardsList = new();

    private void Awake()
    {
        deckViewScreen.SetActive(false);
    }

    public void DisplayDeck(List<Card> deckToDisplay)
    {
        ClearCardsList();

        foreach (var card in deckToDisplay)
        {
            var newCard = Instantiate(cardPrefab, cardContainer.transform);
            
            var cardDisplay = newCard.GetComponent<CardDisplay>();
            
            if (cardDisplay == null)
            {
                Destroy(newCard);
                Debug.LogError("Card prefab is missing CardDisplay.");
                return;
            }
            
            _cardsList.Add(newCard);
            
            var runtimeCard = new RuntimeCard(card);
            var cardMovement = newCard.GetComponent<CardMovement>();
            
            cardMovement.SetCardToDeckMode();

            cardDisplay.runtimeCard = runtimeCard;
        }
        
        deckViewScreen.SetActive(true);
    }

    private void ClearCardsList()
    {
        foreach (var card in _cardsList)
        {
            Destroy(card);
        }
        
        _cardsList.Clear();
    }

    public void OnExitButton()
    {
        deckViewScreen.SetActive(false);
    }
}
