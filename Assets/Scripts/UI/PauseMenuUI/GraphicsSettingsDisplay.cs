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

    private readonly int[] _fpsLimits = {30, 60, 120, -1};

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    public void OnFPSLimitChanged(int index)
    {
        _optionsManager.SetFPSLimit(_fpsLimits[index]);
        RefreshDisplay();
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

        var fpsLimit = _optionsManager.GetFPSLimit();
        var fpsLimitIndex = System.Array.IndexOf(_fpsLimits, fpsLimit);
        
        fpsLimitDropdown.SetValueWithoutNotify(fpsLimitIndex);
    }
}
