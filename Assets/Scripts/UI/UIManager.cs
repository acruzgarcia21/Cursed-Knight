using UnityEngine;

public class UIManager : MonoBehaviour
{
    private TurnManager _turnManager;

    private void Awake()
    {
        _turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void OnEndTurnButtonClicked()
    {
        _turnManager.EndTurn();
    }
    
}
