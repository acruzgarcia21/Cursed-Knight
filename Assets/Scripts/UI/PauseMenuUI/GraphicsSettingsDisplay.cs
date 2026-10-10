using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fpsLimitDropdown;
    [SerializeField] private TMP_Dropdown windowedResolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vSyncToggle;

    private OptionsManager _optionsManager;

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    public void OnVSyncChanged(bool isEnabled)
    {
        _optionsManager.SetVSync(isEnabled);
        RefreshDisplay();
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        _optionsManager.SetFullScreen(isFullscreen);
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        var isFullscreen = _optionsManager.GetFullScreen();

        fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        windowedResolutionDropdown.interactable = !isFullscreen;

        var vSync = _optionsManager.GetVSync();
        
        vSyncToggle.SetIsOnWithoutNotify(vSync);
        fpsLimitDropdown.interactable = !vSync;
    }
}
