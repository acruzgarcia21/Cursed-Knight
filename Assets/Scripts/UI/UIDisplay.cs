using TMPro;
using UnityEngine;

public class UIDisplay : MonoBehaviour
{
    public TMP_Text playerEnergyText;
    public TMP_Text playerCorruptionText;
    public TMP_Text playerCorruptionRoundsText;

    public void UpdatePlayerEnergyText(Player player)
    {
        if (player == null || playerEnergyText == null) return;

        playerEnergyText.text = player.playerEnergy + "/" + player.playerEnergyPerTurn;
    }

    public void UpdatePlayerCorruptionText(Player player)
    {
        if (player == null || playerCorruptionText == null || playerCorruptionRoundsText == null) return;

        playerCorruptionText.text = player.playerCorruption + " / " + player.playerMaxCorruption;
        playerCorruptionRoundsText.text = player.GetStatusDuration(StatusEffect.StatusType.Corruption).ToString();
    }
}
