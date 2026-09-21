using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class CardViewDisplay : MonoBehaviour
{
    [Header("Card View Screen")] 
    [SerializeField] private GameObject cardViewScreen;
    [SerializeField] private GameObject cardContainer;
    [SerializeField] private GameObject cardPrefab;
    
    [SerializeField] private TMP_Text titleText;
    
    [Space(10)] [Header("Card Preview Popup")]
    [SerializeField] private GameObject cardPreviewPopup;
    
    [SerializeField] private RectTransform previewCardPoint;

    private readonly List<GameObject> _cardsList = new();

    private DrawPileManager    _drawPileManager;
    private DiscardManager     _discardManager;
    private ExhaustManager     _exhaustManager;
    private CardRemovalManager _cardRemovalManager;

    private CurrentViewer _currentViewer;
    
    private GameObject _currentPreviewCard;

    private enum CurrentViewer
    {
        Deck,
        DrawPile,
        DiscardPile,
        ExhaustPile,
        CardRemoval
    }

    private void Awake()
    {
        _drawPileManager    = FindAnyObjectByType<DrawPileManager>();
        _discardManager     = FindAnyObjectByType<DiscardManager>();
        _exhaustManager     = FindAnyObjectByType<ExhaustManager>();
        _cardRemovalManager = FindAnyObjectByType<CardRemovalManager>();

        cardViewScreen.SetActive(false);
        cardPreviewPopup.SetActive(false);
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
            cardMovement.OnCardViewClicked += HandleCardViewClicked;

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
            cardMovement.OnCardViewClicked += HandleCardViewClicked;

            cardDisplay.runtimeCard = card;
        }

        cardViewScreen.SetActive(true);
    }

    private void ClearCardsList()
    {
        foreach (var card in _cardsList)
        {
            var cardMovement = card.GetComponent<CardMovement>();
            cardMovement.OnCardViewClicked -= HandleCardViewClicked;
            
            Destroy(card);
        }

        _cardsList.Clear();
    }

    public void OnExitButton()
    {
        cardViewScreen.SetActive(false);
    }

    public void OnCardPopupExitButton()
    {
        cardPreviewPopup.SetActive(false);
        
        Destroy(_currentPreviewCard);
        _currentPreviewCard = null;
    }

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }

    public void SetCurrentViewerToDeck()
    {
        _currentViewer = CurrentViewer.Deck;
    }

    public void SetCurrentViewerToDrawPile()
    {
        _currentViewer = CurrentViewer.DrawPile;
    }

    public void SetCurrentViewerToDiscardPile()
    {
        _currentViewer = CurrentViewer.DiscardPile;
    }

    public void SetCurrentViewerToExhaustPile()
    {
        _currentViewer = CurrentViewer.ExhaustPile;
    }

    public void SetCurrentViewerToCardRemoval()
    {
        _currentViewer = CurrentViewer.CardRemoval;
    }
    
    private void OnEnable()
    {
        _drawPileManager.OnDrawPileChanged   += HandleDrawPileChanged;
        _discardManager.OnDiscardPileChanged += HandleDiscardPileChanged;
        _exhaustManager.OnExhaustPileChanged += HandleExhaustPileChanged;
    }

    private void OnDisable()
    {
        _drawPileManager.OnDrawPileChanged   -= HandleDrawPileChanged;
        _discardManager.OnDiscardPileChanged -= HandleDiscardPileChanged;
        _exhaustManager.OnExhaustPileChanged -= HandleExhaustPileChanged;
    }
    
    private void HandleDrawPileChanged()
    {
        if (_currentViewer != CurrentViewer.DrawPile) return;

        DisplayCards(_drawPileManager.GetDrawPile());
    }

    private void HandleDiscardPileChanged()
    {
        if (_currentViewer != CurrentViewer.DiscardPile) return;

        DisplayCards(_discardManager.GetDiscardPile());
    }
    
    private void HandleExhaustPileChanged()
    {
        if (_currentViewer != CurrentViewer.ExhaustPile) return;

        DisplayCards(_exhaustManager.GetExhaustPile());
    }

    private void HandleCardViewClicked(RuntimeCard runtimeCard)
    {
        if (_currentPreviewCard != null)
        {
            Destroy(_currentPreviewCard);
            _currentPreviewCard = null;
        }

        if (_currentViewer == CurrentViewer.CardRemoval)
        {
            _cardRemovalManager.SelectCard(runtimeCard.cardData);
        }
        else
        {
            cardPreviewPopup.SetActive(true);
        
            _currentPreviewCard = Instantiate(cardPrefab, previewCardPoint);
        
            var cardDisplay = _currentPreviewCard.GetComponent<CardDisplay>();
            var cardMovement = _currentPreviewCard.GetComponent<CardMovement>();
        
            cardMovement.SetCardToCardPopupViewMode();
        
            cardDisplay.runtimeCard = runtimeCard;
        }
    }
}
