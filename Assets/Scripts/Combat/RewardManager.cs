using System.Collections.Generic;
using CursedKnight;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private BattleRewardDisplay battleRewardDisplay;

    [SerializeField] private RewardPoolData rewardPoolData;

    private readonly List<Card> _rewardPool = new();

    private DeckManager _deckManager;

    private bool _rewardCardHasBeenCollected;

    private void Awake()
    {
        _deckManager = FindAnyObjectByType<DeckManager>();
    }

    public void StartRewards()
    {
        battleRewardDisplay.DisplayVictoryScreen();
    }

    public void OnOpenCardRewards()
    {
        if (_rewardCardHasBeenCollected) return;
        
        _rewardCardHasBeenCollected = false;
        
        Debug.Log("Opening Card Rewards");
        battleRewardDisplay.DisplayCardRewardsScreen();
        
        if (rewardPoolData == null) return;
        

        var pool = rewardPoolData.GetRewardPool();
        const int numCardsToAdd = 3;

        _rewardPool.Clear();

        while (_rewardPool.Count < numCardsToAdd)
        {
            var randomCardIndex = Random.Range(0, pool.Count);
            var cardObject = pool[randomCardIndex];

            if (_rewardPool.Contains(cardObject)) continue;

            _rewardPool.Add(cardObject);
        }
        
        battleRewardDisplay.DisplayRewardCard(_rewardPool);
    }

    public void AddSelectedRewardCardToDeck(Card card)
    {
        if (_rewardCardHasBeenCollected) return;
        
        _deckManager.AddCardToDeck(card);

        _rewardCardHasBeenCollected = true;
        
        battleRewardDisplay.CompleteCardRewardSelection();
    }
    
    public void OnContinueRewardCompletion()
    {
        battleRewardDisplay.HideVictoryScreen();

        Debug.Log("Reward Selection Completed!");
    }
}
