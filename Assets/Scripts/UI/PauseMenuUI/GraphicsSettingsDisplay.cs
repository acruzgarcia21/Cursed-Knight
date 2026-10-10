using System.Collections.Generic;
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

    private readonly Vector2Int[] _windowedResolutions =
    {
        new(1280, 720),
        new (1600, 900),
        new (1920, 1080),
        new (2560, 1440),
        new (3840, 2160)
    };

    private readonly List<Vector2Int> _availableWindowedResolutions = new();
    

    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
        
        PopulateResolutionDropdown();
    }
    
    public void OnWindowedResolutionChanged(int index)
    {
        if (index < 0 || index >= _availableWindowedResolutions.Count) return;

        var resolution = _availableWindowedResolutions[index];
        
        _optionsManager.SetWindowedResolution(resolution.x, resolution.y);
        RefreshDisplay();
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

        var windowedResolution = new Vector2Int(_optionsManager.GetWindowedWidth(), _optionsManager.GetWindowedHeight());
        var resolutionIndex = _availableWindowedResolutions.IndexOf(windowedResolution);

        windowedResolutionDropdown.SetValueWithoutNotify(resolutionIndex);
        windowedResolutionDropdown.RefreshShownValue();
    }
    
    private void PopulateResolutionDropdown()
    {
        _availableWindowedResolutions.Clear();

        var options = new List<string>();
        var displayResolution = Screen.currentResolution;

        foreach (var resolution in _windowedResolutions)
        {
            if (resolution.x > displayResolution.width || resolution.y > displayResolution.height) continue;

            _availableWindowedResolutions.Add(resolution);
            options.Add($"{resolution.x} x {resolution.y}");
        }

        windowedResolutionDropdown.ClearOptions();
        windowedResolutionDropdown.AddOptions(options);
    }
}
