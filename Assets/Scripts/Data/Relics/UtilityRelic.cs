using UnityEngine;

[CreateAssetMenu(fileName = "New Utility Relic", menuName = "Relic/UtilityRelic")]

public class UtilityRelic : RelicData
{
    [Space(10)] [Header("Status Effects")] 
    public bool appliesStatus;
    public StatusTargetType statusTargetType;
    public StatusEffect.StatusType statusType;
    public int statusAmount;
    public int statusDuration;
    
    public enum StatusTargetType
    {
        SingleEnemy,
        AllEnemies,
        RandomEnemy,
        Self
    }
    
    [Space(10)] [Header("Energy Reduction")]
    public bool reducesNextAttackEnergy;

    public int energyToReduce;
}
