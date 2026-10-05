using UnityEngine;

public class UIManager : MonoBehaviour
{
    private TurnManager _turnManager;

    private void Awake()
    {
        _turnManager = FindAnyObjectByType<TurnManager>();
    }

    public void OnEndTurnButtonClicked()
    {
        _turnManager.EndTurn();
    }
    
}
