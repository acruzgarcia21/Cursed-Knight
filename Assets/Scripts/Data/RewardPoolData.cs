using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward Pool", menuName = "RewardPoolData")]
public class RewardPoolData : ScriptableObject
{
    [SerializeField] private List<Card> rewardPool = new();
    
    public IReadOnlyList<Card> GetRewardPool()
    {
        return rewardPool;
    }
}
