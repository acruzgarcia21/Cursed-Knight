using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class CardViewDisplay : MonoBehaviour
{
    [SerializeField] private GameObject cardViewScreen;
    [SerializeField] private GameObject cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private TMP_Text titleText;

    private readonly List<GameObject> _cardsList = new();

    private void Awake()
    {
        cardViewScreen.SetActive(false);
    }

    public void DisplayCardDefinitions(List<Card> cardsToDisplay)
    {
        ClearCardsList();

        foreach (var card in cardsToDisplay)
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
            
            cardMovement.SetCardToCardViewMode();

            cardDisplay.runtimeCard = runtimeCard;
        }
        
        cardViewScreen.SetActive(true);
    }

    public void DisplayCards(IReadOnlyList<RuntimeCard> cardsToDisplay)
    {
        ClearCardsList();

        foreach (var card in cardsToDisplay)
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
            
            var cardMovement = newCard.GetComponent<CardMovement>();
            
            cardMovement.SetCardToCardViewMode();

            cardDisplay.runtimeCard = card;
        }
        
        cardViewScreen.SetActive(true);
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
        cardViewScreen.SetActive(false);
    }

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }
}
