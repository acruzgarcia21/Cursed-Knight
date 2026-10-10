using UnityEngine;

public class EndTurnConfirmationDisplay : MonoBehaviour
{
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = FindAnyObjectByType<UIManager>();
    }

    public void OnConfirmClicked()
    {
        _uiManager.ConfirmEndTurn();
    }

    public void OnCancelClicked()
    {
        _uiManager.CancelEndTurn();
    }
}
