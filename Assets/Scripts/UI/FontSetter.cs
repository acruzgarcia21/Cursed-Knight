using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]

public class FontSetter : MonoBehaviour
{
    public string fontClass;
    
    private void OnEnable()
    {
        // Subscribe to the event
        OptionsManager.FontUpdated += SetFont;
    }
    
    private void Start()
    {
        SetFont();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        OptionsManager.FontUpdated -= SetFont;
    }

    private void SetFont()
    {
        var textComponent = GetComponent<TMP_Text>();

        if (textComponent == null) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.OptionsManager == null) return;

        textComponent.font = GameManager.Instance.OptionsManager.GetFontClass(fontClass);
    }
}
