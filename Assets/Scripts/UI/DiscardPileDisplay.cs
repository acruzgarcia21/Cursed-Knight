using TMPro;
using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class DiscardPileDisplay : MonoBehaviour
{
    public TextMeshProUGUI discardCount;
    
    public void UpdateDiscardCount(List<Card> discardPile)
    {
        discardCount.text = discardPile.Count.ToString();
    }
}
