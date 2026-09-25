using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Relic", menuName = "Relic/AttackRelic")]

public class AttackRelic : RelicData
{
    [Space(10)] [Header("Damage")] 
    [SerializeField] private int attackDamage;
    [SerializeField] private int hitCount = 1;
    public override void ResolveEffect(Player player)
    {
        
    }
}
