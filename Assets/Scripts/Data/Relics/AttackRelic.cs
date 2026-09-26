using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Relic", menuName = "Relic/AttackRelic")]

public class AttackRelic : RelicData
{
    [Space(10)] [Header("Damage")] 
    [SerializeField] private int attackDamage;
    
    public override void ResolveEffect(Player player, EnemyManager enemyManager)
    {
        if (attackDamage <= 0) return;
        
        player.StoreRelicDamage(attackDamage);
    }
}
