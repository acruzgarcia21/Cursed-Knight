using TMPro;
using UnityEngine;

public class ExhaustPileDisplay : MonoBehaviour
{
    public TextMeshProUGUI exhaustCount;

    public void UpdateVisuals(int currentExhaustCount)
    {
        if (exhaustCount == null) return;

        exhaustCount.text = currentExhaustCount.ToString();
    }
}
