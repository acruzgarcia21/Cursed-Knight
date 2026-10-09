using UnityEngine;

public class SettingsDisplay : MonoBehaviour
{
    [SerializeField] private AudioSettingsDisplay audioSettingsDisplay;

    private OptionsManager _optionsManager;

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    public void ShowSettingsScreen()
    {
        gameObject.SetActive(true);
        audioSettingsDisplay.RefreshDisplay();
    }

    public void HideSettingsScreen()
    {
        _optionsManager.SaveSettings();
        gameObject.SetActive(false);
    }

    public bool IsSettingsScreenOpen()
    {
        return gameObject.activeSelf;
    }
}
