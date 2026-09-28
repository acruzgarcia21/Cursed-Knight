using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Card", menuName = "Card/Defense")]
public class Defense : Card
{
    [Space(10)] 
    [Header("Card Block")]
    public int cardBlock;

    [Space(10)] 
    [Header("Bonus Block")] 
    public int bonusBlockIfEnemyHasBleed;
    
    [Space(10)] 
    [Header("Status")] 
    public bool appliesStatusToAllEnemies;
    
    [Space(10)]
    [Header("UPGRADE ATTRIBUTES")]

    [Header("Upgrade - Block")]
    [SerializeField] private int upgradedCardBlock;

    [Header("Upgrade - Bonus Block")]
    [SerializeField] private int upgradedBonusBlockIfEnemyHasBleed;

    public int GetCardBlock(bool isUpgraded)
    {
        return isUpgraded ? upgradedCardBlock : cardBlock;
    }

    public int GetBonusBlockIfEnemyHasBleed(bool isUpgraded)
    {
        return isUpgraded ? upgradedBonusBlockIfEnemyHasBleed : bonusBlockIfEnemyHasBleed;
    }
}
