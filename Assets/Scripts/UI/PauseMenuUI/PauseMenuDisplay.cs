using UnityEngine;

public class PauseMenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject abandonRunConfirmationScreen;

    public void ShowPauseScreen()
    {
        pauseMenuPrefab.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pauseMenuPrefab.SetActive(false);
        HideAbandonRunConfirmationScreen();
    }

    public void ShowAbandonRunConfirmationScreen()
    {
        abandonRunConfirmationScreen.SetActive(true);
    }
    
    public void HideAbandonRunConfirmationScreen()
    {
        abandonRunConfirmationScreen.SetActive(false);
    }
}
