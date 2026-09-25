using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Relic", menuName = "Relic/DefenseRelic")]

public class DefenseRelic : RelicData
{
    [Space(10)] [Header("Card Block")]
    [SerializeField] private int relicBlock;

    public override void ResolveEffect(Player player)
    {
        if (player == null) return;
        
        player.GainBlock(relicBlock);
        
        Debug.Log(relicBlock);
        Debug.Log(player.GetCurrentBlockAmount());
    }
}
