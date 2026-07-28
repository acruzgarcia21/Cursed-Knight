using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntentEntryDisplay : MonoBehaviour
{
    public Image intentIcon;
    public TMP_Text intentText;

    public void DisplayIntent(Sprite icon, string text)
    {
        if (icon == null) return;
        gameObject.SetActive(true);

        intentIcon.sprite = icon;
        intentText.text = text;
    }

    public void Clear()
    {
        intentIcon.sprite = null;
        intentText.text = "";
        gameObject.SetActive(false);
    }
}
