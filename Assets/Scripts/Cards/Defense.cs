using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Card", menuName = "Card/Defense")]
public class Defense : Card
{
    [Space(10)] [Header("Card Block")]
    public int cardBlock;
}
