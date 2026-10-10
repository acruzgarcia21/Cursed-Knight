using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject _endTurnConfirmationScreen;

    private TurnManager _turnManager;
    private CardPlayManager _cardPlayManager;
    private OptionsManager _optionsManager;

    private void Awake()
    {
        _endTurnConfirmationScreen = FindAnyObjectByType<EndTurnConfirmationDisplay>(FindObjectsInactive.Include).gameObject;
        _turnManager = FindAnyObjectByType<TurnManager>();
        _cardPlayManager = FindAnyObjectByType<CardPlayManager>();
        _optionsManager = FindAnyObjectByType<OptionsManager>();
        _endTurnConfirmationScreen.SetActive(false);
    }

    public void OnEndTurnButtonClicked()
    {
        if (!_turnManager.IsCombatActive() || _turnManager.IsResolvingTurn() || Time.timeScale == 0f) return;
        if (_cardPlayManager.IsResolvingCard()) return;
        if (_turnManager.GetTurnState() != TurnManager.TurnState.Player) return;

        if (_optionsManager.GetConfirmEndTurn())
        {
            _endTurnConfirmationScreen.SetActive(true);
            return;
        }

        _turnManager.EndTurn();
    }

    public void ConfirmEndTurn()
    {
        if (Time.timeScale == 0f) return;

        _endTurnConfirmationScreen.SetActive(false);
        _turnManager.EndTurn();
    }

    public void CancelEndTurn()
    {
        _endTurnConfirmationScreen.SetActive(false);
    }
}
