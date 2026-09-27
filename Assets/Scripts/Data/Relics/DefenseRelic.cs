using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Relic", menuName = "Relic/DefenseRelic")]

public class DefenseRelic : RelicData
{
    [Space(10)] [Header("Card Block")]
    [SerializeField] private int relicBlock;

    public override void ResolveEffect(Player player, EnemyManager enemyManager)
    {
        if (player == null) return;
        
        if (relicBlock > 0) player.GainBlock(relicBlock);
        
        if (corruptionCost > 0) player.GainCorruption(corruptionCost);
    }
}
