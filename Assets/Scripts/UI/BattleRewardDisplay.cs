using UnityEngine;

public class BattleRewardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject rewardsContainer;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject cardsRewardScreen;

    private void Awake()
    {
        victoryScreen.SetActive(false);
    }

    public void DisplayVictoryScreen()
    {
        victoryScreen.SetActive(true);
        cardsRewardScreen.SetActive(false);
    }

    public void HideVictoryScreen()
    {
        victoryScreen.SetActive(false);
    }

    public void DisplayCardRewardsScreen()
    {
        rewardsContainer.SetActive(false);
        continueButton.SetActive(false);
        victoryText.SetActive(false);
        cardsRewardScreen.SetActive(true);
    }
}
