using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class BattleRewardDisplay : MonoBehaviour
{
    public event System.Action OnRewardSelectionCompleted;
    
    [Header("Reward Screen Attributes")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject rewardsContainer;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject cardsRewardScreen;
    [SerializeField] private GameObject cardRewardsButton;

    [Space(10)] [Header("Card Reward Attributes")]
    [SerializeField] private List<RectTransform> rewardCardPoints;

    [SerializeField] private GameObject cardPrefab;

    private readonly List<GameObject> _rewardCardObjects = new();
    
    private void Awake()
    {
        victoryScreen.SetActive(false);
    }

    public void DisplayVictoryScreen()
    {
        victoryScreen.SetActive(true);
        cardRewardsButton.SetActive(true);
        cardsRewardScreen.SetActive(false);
    }
    
    public void DisplayCardRewardsScreen()
    {
        rewardsContainer.SetActive(false);
        continueButton.SetActive(false);
        victoryText.SetActive(false);
        cardsRewardScreen.SetActive(true);
    }

    public void DisplayRewardCard(List<Card> cardPool)
    {
        if (cardPrefab == null) return;
        
        for (var i = 0; i < cardPool.Count; i++)
        {
            var newCard = Instantiate(
                cardPrefab, 
                rewardCardPoints[i].position, 
                Quaternion.identity, 
                rewardCardPoints[i]);
            
            _rewardCardObjects.Add(newCard);
            
            var cardDisplay = newCard.GetComponent<CardDisplay>();
            var cardMovement = newCard.GetComponent<CardMovement>();
            
            cardMovement.SetCardToRewardMode();

            if (cardDisplay == null)
            {
                Destroy(newCard);
                Debug.LogError("Card prefab is missing CardDisplay.");
                return;
            }

            var runtimeCard = new RuntimeCard(cardPool[i]);

            cardDisplay.runtimeCard = runtimeCard;
        }
    }

    public void CompleteCardRewardSelection()
    {
        cardsRewardScreen.SetActive(false);
        rewardsContainer.SetActive(true);
        continueButton.SetActive(true);
        victoryText.SetActive(true);
        cardRewardsButton.SetActive(false);

        CleanUpCardSelection();
    }

    public void OnSkipRewardCardSelection()
    {
        CompleteCardRewardSelection();
    }

    public void ContinueAfterRewards()
    {
        HideVictoryScreen();
        
        OnRewardSelectionCompleted?.Invoke();
    }
    private void HideVictoryScreen()
    {
        victoryScreen.SetActive(false);
    }

    private void CleanUpCardSelection()
    {
        foreach (var card in _rewardCardObjects)
        {
            Destroy(card);
        }
        
        _rewardCardObjects.Clear();
    }
}
