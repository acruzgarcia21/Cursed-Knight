using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text playerEnergyText;
    public TMP_Text playerCorruptionText;
    public TMP_Text playerCorruptionRoundsText;
    
    private Player _player;
    private TurnManager _turnManager;

    private void Awake()
    {
        _turnManager = FindFirstObjectByType<TurnManager>();
        _player      = FindFirstObjectByType<Player>();
    }

    public void OnEndTurnButtonClicked()
    {
        _turnManager.EndTurn();
    }

    public void UpdatePlayerEnergyText()
    {
        playerEnergyText.text = _player.playerEnergy + "/" + _player.playerMaxEnergy;
    }

    public void UpdatePlayerCorruptionText()
    {
        playerCorruptionText.text = _player.playerCorruption + " / " + _player.playerMaxCorruption;
        playerCorruptionRoundsText.text = _player.corruptionDebuffTurns.ToString();
    }
}
