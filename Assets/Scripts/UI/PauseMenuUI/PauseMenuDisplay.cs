using UnityEngine;

public class PauseMenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPrefab;

    public void ShowPauseScreen()
    {
        pauseMenuPrefab.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pauseMenuPrefab.SetActive(false);
    }
}
