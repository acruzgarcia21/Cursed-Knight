using UnityEngine;

public class BattleRewardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen;


    private void Awake()
    {
        victoryScreen.SetActive(false);
    }

    public void DisplayVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    public void HideVictoryScreen()
    {
        victoryScreen.SetActive(false);
    }
}
