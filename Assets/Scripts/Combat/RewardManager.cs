using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private BattleRewardDisplay battleRewardDisplay;

    public void StartRewards()
    {
        battleRewardDisplay.DisplayVictoryScreen();
    }

    public void OnOpenCardRewards()
    {
        Debug.Log("Opening Card Rewards");
        battleRewardDisplay.DisplayCardRewardsScreen();
    }
}
