using UnityEngine;
using UnityEngine.UI;

public class AccessibilitySettingsDisplay : MonoBehaviour
{
    [SerializeField] private Toggle reduceMotionToggle;
    [SerializeField] private Toggle confirmEndTurnToggle;

    private OptionsManager _optionsManager;

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    public void OnReduceMotionChanged(bool isEnabled)
    {
        _optionsManager.SetReduceMotion(isEnabled);
        RefreshDisplay();
    }

    public void OnConfirmEndTurnChanged(bool isEnabled)
    {
        _optionsManager.SetConfirmEndTurn(isEnabled);
        RefreshDisplay();
    }

    public void ResetToDefaults()
    {
        _optionsManager.SetReduceMotion(false);
        _optionsManager.SetConfirmEndTurn(false);
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        reduceMotionToggle.SetIsOnWithoutNotify(_optionsManager.GetReduceMotion());
        confirmEndTurnToggle.SetIsOnWithoutNotify(_optionsManager.GetConfirmEndTurn());
    }
}
