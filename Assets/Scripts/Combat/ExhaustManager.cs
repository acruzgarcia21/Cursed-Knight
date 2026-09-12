using System.Collections.Generic;
using UnityEngine;

public class ExhaustManager : MonoBehaviour
{
    public event System.Action OnExhaustPileChanged;
    
    private ExhaustPileDisplay _exhaustPileDisplay;
    private readonly List<RuntimeCard> _exhaustPile = new();
    
    private void Awake()
    {
        _exhaustPileDisplay = FindAnyObjectByType<ExhaustPileDisplay>();
        UpdateExhaustCount();
    }
    
    public void AddToExhaustPile(RuntimeCard runtimeCard)
    {
        if (runtimeCard == null) return;

        _exhaustPile.Add(runtimeCard);
        UpdateExhaustCount();
        
        OnExhaustPileChanged?.Invoke();
    }

    public IReadOnlyList<RuntimeCard> GetExhaustPile()
    {
        return _exhaustPile;
    }
    
    private void UpdateExhaustCount()
    {
        if (_exhaustPileDisplay == null) return;

        _exhaustPileDisplay.UpdateExhaustCount(_exhaustPile.Count);
    }
}
