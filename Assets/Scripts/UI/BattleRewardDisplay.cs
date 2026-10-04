using System.Collections;
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
    
    [Space(10)] [Header("Card Reward Spawn Animation")]
    [SerializeField] private float revealDuration = 0.25f;

    [SerializeField] private float startingHeight = 60f;

    [Space(10)] [Header("Relic Reward Attributes")] 
    [SerializeField] private GameObject relicRewardsButton;

    [SerializeField] private RelicRewardDisplay relicRewardDisplay;
    
    private void Awake()
    {
        victoryScreen.SetActive(false);
    }

    public void DisplayVictoryScreen(RelicData relicData)
    {
        victoryScreen.SetActive(true);
        cardRewardsButton.SetActive(true);
        cardsRewardScreen.SetActive(false);

        relicRewardsButton.SetActive(relicData != null);
        relicRewardDisplay.DisplayRelic(relicData);
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

            var rectTransform = newCard.GetComponent<RectTransform>();
            
            var canvasGroup = newCard.GetComponent<CanvasGroup>();
            
            var finalPosition = newCard.transform.localPosition;
            
            var startingPosition = new Vector3(finalPosition.x, finalPosition.y + startingHeight, finalPosition.z);

            newCard.transform.localPosition = startingPosition;
            
            canvasGroup.alpha = 0;
            
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
            
            StartCoroutine(
                RevealRewardCardAnimation(rectTransform, canvasGroup, finalPosition, i * 0.08f)
            );
        }
    }

    public IEnumerator RevealRewardCardAnimation(
        RectTransform rectTransform,
        CanvasGroup canvasGroup,
        Vector3 finalPosition,
        float delay
    )
    {
        yield return new WaitForSeconds(delay);

        var startingPosition = rectTransform.localPosition;
        var elapsedTime = 0f;

        while (elapsedTime < revealDuration)
        {
            elapsedTime += Time.deltaTime;
            var progress = Mathf.Clamp01(elapsedTime / revealDuration);

            rectTransform.localPosition = Vector3.Lerp(startingPosition, finalPosition, progress);
            canvasGroup.alpha = progress;

            yield return null;
        }

        rectTransform.localPosition = finalPosition;
        canvasGroup.alpha = 1f;
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
