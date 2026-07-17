using System.Collections.Generic;
using UnityEngine;

public class ExhaustManager : MonoBehaviour
{
    private ExhaustPileDisplay _exhaustPileDisplay;
    private readonly List<RuntimeCard> _exhaustPile = new();
    
    private void Awake()
    {
        _exhaustPileDisplay = FindFirstObjectByType<ExhaustPileDisplay>();
        UpdateExhaustCount();
    }
    
    public void AddToExhaustPile(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return;

        _exhaustPile.Add(runtimeCard);
        UpdateExhaustCount();
    }

    
    private void UpdateExhaustCount()
    {
        if (_exhaustPileDisplay == null) return;

        _exhaustPileDisplay.UpdateExhaustCount(_exhaustPile.Count);
    }
}
