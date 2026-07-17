using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Card/Attack")]
public class Attack : Card
{
    [Space(10)] [Header("Damage")]
    public int cardDamage;
    public int hitCount = 1;
}
