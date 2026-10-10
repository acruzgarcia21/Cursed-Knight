using UnityEngine;

public class SettingsDisplay : MonoBehaviour
{
    [SerializeField] private AudioSettingsDisplay audioSettingsDisplay;
    [SerializeField] private GraphicsSettingsDisplay graphicsSettingsDisplay;
    [SerializeField] private AccessibilitySettingsDisplay accessibilitySettingsDisplay;
    [SerializeField] private RectTransform backButton;

    [SerializeField] private float backButtonOffsetWithReset = 230f;

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
        backButton.anchoredPosition = new Vector2(backButtonOffsetWithReset, backButton.anchoredPosition.y);
        audioSettingsDisplay.RefreshDisplay();
    }

    public void ShowGraphicsPanel()
    {
        HideAllPanels();
        graphicsPanel.SetActive(true);
        backButton.anchoredPosition = new Vector2(backButtonOffsetWithReset, backButton.anchoredPosition.y);
        graphicsSettingsDisplay.RefreshDisplay();
    }

    public void ShowControlsPanel()
    {
        HideAllPanels();
        controlsPanel.SetActive(true);
        backButton.anchoredPosition = new Vector2(0f, backButton.anchoredPosition.y);
    }

    public void ShowAccessibilityPanel()
    {
        HideAllPanels();
        accessibilityPanel.SetActive(true);
        accessibilitySettingsDisplay.RefreshDisplay();
        backButton.anchoredPosition = new Vector2(backButtonOffsetWithReset, backButton.anchoredPosition.y);
    }

    private void HideAllPanels()
    {
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        accessibilityPanel.SetActive(false);
    }
}
