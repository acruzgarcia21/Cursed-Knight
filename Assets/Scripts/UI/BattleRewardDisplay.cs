using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class BattleRewardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject rewardsContainer;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject cardsRewardScreen;

    [SerializeField] private List<RectTransform> rewardCardPoints;

    [SerializeField] private GameObject cardPrefab;
    
    private void Awake()
    {
        victoryScreen.SetActive(false);
    }

    public void DisplayVictoryScreen()
    {
        victoryScreen.SetActive(true);
        cardsRewardScreen.SetActive(false);
    }

    public void HideVictoryScreen()
    {
        victoryScreen.SetActive(false);
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
            
            var cardDisplay = newCard.GetComponent<CardDisplay>();

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
}
