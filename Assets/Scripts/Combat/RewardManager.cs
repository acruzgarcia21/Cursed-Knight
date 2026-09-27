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
    private RelicManager _relicManager;

    private bool _rewardCardHasBeenCollected;

    [Space(10)] [Header("Relic Rewards")]
    [SerializeField] private float relicRewardChance = 0.2f;

    private int _relicsAwardedThisAct;
    private const int MaxPossibleRelicsThisAct = 2;

    private void Awake()
    {
        _deckManager  = FindAnyObjectByType<DeckManager>();
        _relicManager = FindAnyObjectByType<RelicManager>();
    }

    public void ActSetup()
    {
        _relicsAwardedThisAct = 0;
    }

    public void StartRewards(Map.MapNodeType currentNode, bool isFinalStage)
    {
        _rewardCardHasBeenCollected = false;

        RelicData selectedRelic = null;

        if (currentNode == Map.MapNodeType.Boss)
        {
            selectedRelic = _relicManager.SelectRelic();
        }
        else if (isFinalStage && _relicsAwardedThisAct == 0 || RollRelicSelection())
        {
            selectedRelic = _relicManager.SelectRelic();

            if (selectedRelic != null)
            {
                _relicsAwardedThisAct++;
            }
        }

        battleRewardDisplay.DisplayVictoryScreen(selectedRelic);
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

    private bool RollRelicSelection()
    {
        if (_relicsAwardedThisAct >= MaxPossibleRelicsThisAct) return false;
        
        var randomFloat = Random.Range(0f, 1f);
        return randomFloat < relicRewardChance;
    }
}
