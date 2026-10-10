using TMPro;
using UnityEngine;

public class DiscardPileDisplay : MonoBehaviour
{
    public TextMeshProUGUI discardCount;

    public void UpdateVisuals(int count)
    {
        if (discardCount == null) return;

        discardCount.text = count.ToString();
    }
}