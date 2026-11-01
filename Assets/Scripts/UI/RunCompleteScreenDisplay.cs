using UnityEngine;

public class RunCompleteScreenDisplay : MonoBehaviour
{
    [SerializeField] private GameObject runCompleteScreen;

    private bool _isLoading;
    
    public void DisplayRunCompleteScreen()
    {
        runCompleteScreen.SetActive(true);
    }
    
    public void HideRunCompleteScreen()
    {
        runCompleteScreen.SetActive(false);
    }

    public void OnViewCreditsClicked()
    {
        if (_isLoading) return;

        _isLoading = true;
        Loader.Load(Loader.Scene.CreditsScene);
    }
}
