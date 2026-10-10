using UnityEngine;

public class SettingsDisplay : MonoBehaviour
{
    [SerializeField] private AudioSettingsDisplay audioSettingsDisplay;
    [SerializeField] private GraphicsSettingsDisplay graphicsSettingsDisplay;

    [Header("Panel Tabs")] 
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject accessibilityPanel;
    
    private OptionsManager _optionsManager;

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    public void ShowSettingsScreen()
    {
        gameObject.SetActive(true);
        ShowAudioPanel();
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
    

    public void ShowAudioPanel()
    {
        HideAllPanels();
        audioPanel.SetActive(true);
        audioSettingsDisplay.RefreshDisplay();
    }

    public void ShowGraphicsPanel()
    {
        HideAllPanels();
        graphicsPanel.SetActive(true);
        graphicsSettingsDisplay.RefreshDisplay();
    }

    public void ShowControlsPanel()
    {
        HideAllPanels();
        controlsPanel.SetActive(true);
    }

    public void ShowAccessibilityPanel()
    {
        HideAllPanels();
        accessibilityPanel.SetActive(true);
    }

    private void HideAllPanels()
    {
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        accessibilityPanel.SetActive(false);
    }
}
