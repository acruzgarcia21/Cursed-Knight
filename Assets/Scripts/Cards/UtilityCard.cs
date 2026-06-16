using UnityEngine;
using CursedKnight;

[CreateAssetMenu(fileName = "New Utility Card", menuName = "Card/UtilityCard")]
public class UtilityCard : Card
{
    public int cardHealthGain;
    public int cardEnergyGain;
    public int cardsToDraw;
}
