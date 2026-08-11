using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Card", menuName = "Card/Defense")]
public class Defense : Card
{
    [Space(10)] [Header("Card Block")]
    public int cardBlock;

    [Space(10)] [Header("Bonus Block")] 
    public int bonusBlockIfEnemyHasBleed;
    
    [Space(10)] [Header("Status")] 
    public bool appliesStatusToAllEnemies;
}
