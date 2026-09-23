using UnityEngine;

public class RunCompleteScreenDisplay : MonoBehaviour
{
    [SerializeField] private GameObject runCompleteScreen;
    
    private void Awake()
    {
        runCompleteScreen.SetActive(false);
    }

    public void DisplayRunCompleteScreen()
    {
        runCompleteScreen.SetActive(true);
    }
    
    public void HideRunCompleteScreen()
    {
        runCompleteScreen.SetActive(false);
    }
}
