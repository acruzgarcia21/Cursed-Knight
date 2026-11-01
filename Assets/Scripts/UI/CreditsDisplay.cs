using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ScrollRect creditsScrollRect;
    [SerializeField] private TMP_Text creditsText;

    private bool _isLeaving;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        creditsScrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnReturnToMainMenuClicked()
    {
        if (_isLeaving) return;

        _isLeaving = true;
        Time.timeScale = 1f;
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        var linkIndex = TMP_TextUtilities.FindIntersectingLink(creditsText, eventData.position, eventData.pressEventCamera);
        if (linkIndex < 0) return;

        var url = creditsText.textInfo.linkInfo[linkIndex].GetLinkID();
        if (!url.StartsWith("https://", System.StringComparison.Ordinal)) return;

        Application.OpenURL(url);
    }
}
