using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour
{
    public TMP_Text playerEnergyText;
    public TMP_Text playerCorruptionText;
    public TMP_Text playerCorruptionRoundsText;

    [SerializeField] private GameObject corruptionRounds;
    
    [SerializeField] private Image playerCorruptionFill;

    private void Awake()
    {
        corruptionRounds.SetActive(false);
    }

    public void UpdatePlayerEnergyText(Player player)
    {
        if (player == null || playerEnergyText == null) return;

        playerEnergyText.text = player.playerEnergy + "/" + player.playerEnergyPerTurn;
    }

    public void UpdatePlayerCorruptionText(Player player)
    {
        if (player == null || playerCorruptionText == null || playerCorruptionRoundsText == null) return;

        playerCorruptionText.text = player.playerCorruption + " / " + player.playerMaxCorruption;

        if (player.HasStatus(StatusEffect.StatusType.Corruption))
        {
            playerCorruptionRoundsText.text = 
                "Rounds: " +  player.GetStatusDuration(StatusEffect.StatusType.Corruption);
            
            corruptionRounds.SetActive(true);
        }
        else
        {
            corruptionRounds.SetActive(false);
        }

        var corruptionPercent = (float)player.playerCorruption / player.playerMaxCorruption;

        playerCorruptionFill.fillAmount = corruptionPercent;
    }
}
