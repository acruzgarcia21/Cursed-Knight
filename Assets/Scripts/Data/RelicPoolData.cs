using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Relic Pool", menuName = "RelicPoolData")]
public class RelicPoolData : ScriptableObject
{
    [SerializeField] private List<RelicData> rewardPool = new();
    
    public IReadOnlyList<RelicData> GetRewardPool()
    {
        return rewardPool;
    }
}
