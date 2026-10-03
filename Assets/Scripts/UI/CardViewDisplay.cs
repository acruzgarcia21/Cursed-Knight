using System.Collections.Generic;
using CursedKnight;
using TMPro;
using UnityEngine;

public class CardViewDisplay : MonoBehaviour
{
    private System.Action _pendingCardManipulation;
    
    [Header("Card View Screen")] [SerializeField]
    private GameObject cardViewScreen;

    [SerializeField] private GameObject cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private TMP_Text titleText;

    [Space(10)] [Header("Card Preview Popup")] [SerializeField]
    private GameObject cardPreviewPopup;

    [SerializeField] private RectTransform previewCardPoint;

    [Space(10)] [Header("card Manipulation Popup")] [SerializeField]
    private GameObject cardManipulationPopup;

    [SerializeField] private GameObject backgroundDim;
    [SerializeField] private RectTransform currentCardPoint;
    [SerializeField] private RectTransform upgradedCardPoint;

    private GameObject _currentUpgradePreviewCard;
    private GameObject _upgradedUpgradePreviewCard;
    
    private readonly List<GameObject> _cardsList = new();

    private DrawPileManager    _drawPileManager;
    private DiscardManager     _discardManager;
    private ExhaustManager     _exhaustManager;
    private CardRemovalManager _cardRemovalManager;
    private CardUpgradeManager _cardUpgradeManager;
    private AudioManager _audioManager;

    private CurrentViewer _currentViewer;

    private GameObject _currentPreviewCard;

    private CardMovement _currentCard;

    private enum CurrentViewer
    {
        Deck,
        DrawPile,
        DiscardPile,
        ExhaustPile,
        CardRemoval,
        CardUpgrade
    }

    private void Awake()
    {
        _drawPileManager    = FindAnyObjectByType<DrawPileManager>();
        _discardManager     = FindAnyObjectByType<DiscardManager>();
        _exhaustManager     = FindAnyObjectByType<ExhaustManager>();
        _cardRemovalManager = FindAnyObjectByType<CardRemovalManager>();
        _cardUpgradeManager = FindAnyObjectByType<CardUpgradeManager>();
        _audioManager       = FindAnyObjectByType<AudioManager>();

        cardViewScreen.SetActive(false);
        cardPreviewPopup.SetActive(false);
        cardManipulationPopup.SetActive(false);
        backgroundDim.SetActive(false);
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

        if (!cardViewScreen.activeSelf) _audioManager.PlayCardViewOpenSound();
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
        if (cardViewScreen.activeSelf) _audioManager.PlayCardViewCloseSound();
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

    public void SetCurrentViewerToUpgradeCard()
    {
        _currentViewer = CurrentViewer.CardUpgrade;
    }

    private void ShowCardManipulationPopup(System.Action confirmationAction)
    {
        _pendingCardManipulation = confirmationAction;
        cardManipulationPopup.SetActive(true);
    }
    
    public void OnCardManipulationConfirm()
    {
        _pendingCardManipulation?.Invoke();
    }
    
    private void OnEnable()
    {
        if (_drawPileManager != null)
            _drawPileManager.OnDrawPileChanged += HandleDrawPileChanged;

        if (_discardManager != null)
            _discardManager.OnDiscardPileChanged += HandleDiscardPileChanged;

        if (_exhaustManager != null)
            _exhaustManager.OnExhaustPileChanged += HandleExhaustPileChanged;

        if (_cardRemovalManager != null)
            _cardRemovalManager.OnCardRemoved += HandleCardManipulation;

        if (_cardUpgradeManager != null)
            _cardUpgradeManager.OnCardUpgraded += HandleCardManipulation;
    }

    private void OnDisable()
    {
        if (_drawPileManager != null)
            _drawPileManager.OnDrawPileChanged -= HandleDrawPileChanged;

        if (_discardManager != null)
            _discardManager.OnDiscardPileChanged -= HandleDiscardPileChanged;

        if (_exhaustManager != null)
            _exhaustManager.OnExhaustPileChanged -= HandleExhaustPileChanged;

        if (_cardRemovalManager != null)
            _cardRemovalManager.OnCardRemoved -= HandleCardManipulation;

        if (_cardUpgradeManager != null)
            _cardUpgradeManager.OnCardUpgraded -= HandleCardManipulation;
    }

    public void OnCardManipulationCancel()
    {
        cardManipulationPopup.SetActive(false);

        if (_currentCard != null)
        {
            _currentCard.ReturnToIdleState();
            _currentCard = null;
        }

        _cardRemovalManager.ClearSelectedCard();
        _cardUpgradeManager.ClearSelectedCard();

        ClearUpgradePreview();

        _pendingCardManipulation = null;
    }
    
    private void HandleDrawPileChanged()
    {
        if (!cardViewScreen.activeSelf) return;
        if (_currentViewer != CurrentViewer.DrawPile) return;

        DisplayCards(_drawPileManager.GetDrawPile());
    }

    private void HandleDiscardPileChanged()
    {
        if (!cardViewScreen.activeSelf) return;
        if (_currentViewer != CurrentViewer.DiscardPile) return;

        DisplayCards(_discardManager.GetDiscardPile());
    }
    
    private void HandleExhaustPileChanged()
    {
        if (!cardViewScreen.activeSelf) return;
        if (_currentViewer != CurrentViewer.ExhaustPile) return;

        DisplayCards(_exhaustManager.GetExhaustPile());
    }

    private void HandleCardViewClicked(RuntimeCard runtimeCard, CardMovement cardMovement)
    {
        if (_currentPreviewCard != null)
        {
            Destroy(_currentPreviewCard);
            _currentPreviewCard = null;
        }

        if (_currentViewer == CurrentViewer.CardRemoval)
        {
            if (_currentCard != null)
                _currentCard.ReturnToIdleState();

            _currentCard = cardMovement;
            _currentCard.SetCardToRemovalSelectedState();

            _cardRemovalManager.SelectCard(runtimeCard);
            ShowCardManipulationPopup(_cardRemovalManager.ResolveCardRemovalConfirmation);
            return;
        }

        if (_currentViewer == CurrentViewer.CardUpgrade)
        {
            if (runtimeCard.isUpgraded) return;

            if (_currentCard != null)
                _currentCard.ReturnToIdleState();

            _currentCard = cardMovement;
            _currentCard.SetCardToUpgradeSelectedState();

            _cardUpgradeManager.SelectCard(runtimeCard);

            ShowCardManipulationPopup(_cardUpgradeManager.ResolveCardUpgradeConfirmation);
            ShowUpgradePreview(runtimeCard);
            return;
        }

        cardPreviewPopup.SetActive(true);

        _currentPreviewCard = Instantiate(cardPrefab, previewCardPoint);

        var cardDisplay = _currentPreviewCard.GetComponent<CardDisplay>();
        var previewCardMovement = _currentPreviewCard.GetComponent<CardMovement>();

        previewCardMovement.SetCardToCardPopupViewMode();

        cardDisplay.runtimeCard = runtimeCard;
    }
    
    private void HandleCardManipulation()
    {
        ClearUpgradePreview();

        cardManipulationPopup.SetActive(false);
        if (cardViewScreen.activeSelf) _audioManager.PlayCardViewCloseSound();
        cardViewScreen.SetActive(false);
        backgroundDim.SetActive(false);

        _currentCard = null;
        _pendingCardManipulation = null;
    }
    
    private void ShowUpgradePreview(RuntimeCard runtimeCard)
    {
        ClearUpgradePreview();

        backgroundDim.SetActive(true);
        
        _currentUpgradePreviewCard = Instantiate(cardPrefab, currentCardPoint);

        var currentCardDisplay = _currentUpgradePreviewCard.GetComponent<CardDisplay>();
        var currentCardMovement = _currentUpgradePreviewCard.GetComponent<CardMovement>();

        currentCardDisplay.runtimeCard = runtimeCard;
        currentCardMovement.SetCardToCardPopupViewMode();

        _upgradedUpgradePreviewCard = Instantiate(cardPrefab, upgradedCardPoint);

        var upgradedCardDisplay = _upgradedUpgradePreviewCard.GetComponent<CardDisplay>();
        var upgradedCardMovement = _upgradedUpgradePreviewCard.GetComponent<CardMovement>();

        upgradedCardDisplay.runtimeCard = runtimeCard;
        upgradedCardMovement.SetCardToCardPopupViewMode();
        upgradedCardDisplay.SetToUpgradePreview();
    }
    
    private void ClearUpgradePreview()
    {
        if (_currentUpgradePreviewCard != null)
        {
            Destroy(_currentUpgradePreviewCard);
            _currentUpgradePreviewCard = null;
        }

        if (_upgradedUpgradePreviewCard != null)
        {
            Destroy(_upgradedUpgradePreviewCard);
            _upgradedUpgradePreviewCard = null;
        }
    }
}
