using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Card/Attack")]
public class Attack : Card
{
    [Space(10)] 
    [Header("Damage")]
    public int cardDamage;
    public int hitCount = 1;

    [Space(10)] 
    [Header("Corruption Scaling")]
    public bool scalesWithCorruption;
    public int corruptionDamagePerPoint;
    
    [Space(10)]
    [Header("UPGRADE ATTRIBUTES")]

    [Header("Upgrade - Damage")]
    [SerializeField] private int upgradedCardDamage;
    [SerializeField] private int upgradedHitCount;

    [Header("Upgrade - Corruption Scaling")]
    [SerializeField] private int upgradedCorruptionDamagePerPoint;
}
