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
    
    /*[Space(10)] [Header("Energy Reduction")]
    public bool reducesNextAttackEnergy;
    public int energyToReduce;*/

    [Space(10)] [Header("Relic Cost")] 
    public int corruptionCost;

    [Space(10)] [Header("Heal")] 
    public int hpToGain;

    [Space(10)] [Header("Energy")] 
    public int energyToGain;
    
    public override void ResolveEffect(Player player)
    {
        if (appliesStatus && statusTargetType == StatusTargetType.Self)
        {
            player.ApplyStatus(new StatusEffect
            {
                statusType = statusType,
                amount = statusAmount,
                duration = statusDuration
            });
        }
        
        if (hpToGain > 0)
        {
            player.Heal(hpToGain);
        }

        if (energyToGain > 0)
        {
            player.GainEnergy(energyToGain);
        }
            
        if (corruptionCost > 0)
        {
            player.GainCorruption(corruptionCost);
        }
    }
}
