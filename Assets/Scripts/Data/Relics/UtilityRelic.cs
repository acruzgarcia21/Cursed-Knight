using UnityEngine;

[CreateAssetMenu(fileName = "New Utility Relic", menuName = "Relic/UtilityRelic")]

public class UtilityRelic : RelicData
{
    [Space(10)] [Header("Status Effects")] 
    [SerializeField] private bool appliesStatus;
    
    [SerializeField] private StatusTargetType statusTargetType;
    [SerializeField] private StatusEffect.StatusType statusType;
    
    [SerializeField] private int statusAmount;
    [SerializeField] private int statusDuration;
    
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
    
    
    [Space(10)] [Header("Heal")]
    [SerializeField] private bool conditionalHeal;
    [SerializeField] private int hpToGain;

    [Space(10)] [Header("Energy")] 
    [SerializeField] private bool conditionalEnergy;
    [SerializeField] private int energyToGain;
    [SerializeField] private int corruptionRequiredForEnergy;
    
    
    public override void ResolveEffect(Player player, EnemyManager enemyManager)
    {
        if (appliesStatus)
        {
            switch (statusTargetType)
            {
                case StatusTargetType.AllEnemies:
                    var enemies = enemyManager.GetLivingEnemies();
                    foreach (var enemy in enemies)
                    {
                        enemy.ApplyStatus(new StatusEffect
                        {
                            statusType = statusType,
                            amount = statusAmount,
                            duration = statusDuration
                        });
                    }
                    
                    break;
                    
                case StatusTargetType.Self:
                    player.ApplyStatus(new StatusEffect
                    {
                        statusType = statusType,
                        amount = statusAmount,
                        duration = statusDuration
                    });
                    
                    break;
            }
        }
        
        if (hpToGain > 0 && conditionalHeal)
        {
            var amountToHeal = hpToGain * enemyManager.GetNumEnemiesKilled();
            
            player.Heal(amountToHeal);
        }
        
        if (hpToGain > 0 && !conditionalHeal)
        {
            player.Heal(hpToGain);
        }

        if (energyToGain > 0 && conditionalEnergy)
        {
            if (player.playerCorruption >= corruptionRequiredForEnergy)
            {
                player.GainEnergy(energyToGain);
            }
        }
        
        if (energyToGain > 0 && !conditionalEnergy)
        {
            player.GainEnergy(energyToGain);
        }
            
        if (corruptionCost > 0)
        {
            player.GainCorruption(corruptionCost);
        }
        
    }
}
