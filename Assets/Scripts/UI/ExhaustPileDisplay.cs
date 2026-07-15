using TMPro;
using UnityEngine;

public class ExhaustPileDisplay : MonoBehaviour
{
    public TextMeshProUGUI exhaustCount;

    public void UpdateExhaustCount(int count)
    {
        if (exhaustCount == null) return;

        exhaustCount.text = count.ToString();
    }
}
