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
        _player      = FindFirstObjectByType<Player>();
        _turnManager = FindFirstObjectByType<TurnManager>();
    }

    private void Start()
    {
        UpdatePlayerEnergyText();
        UpdatePlayerCorruptionText();
    }

    public void OnEndTurnButtonClicked()
    {
        _turnManager.EndTurn();
    }

    public void UpdatePlayerEnergyText()
    {
        if (_player == null)
            _player = FindFirstObjectByType<Player>();

        if (_player == null || playerEnergyText == null) return;

        playerEnergyText.text = _player.playerEnergy + "/" + _player.playerEnergyPerTurn;
    }

    public void UpdatePlayerCorruptionText()
    {
        if (_player == null)
            _player = FindFirstObjectByType<Player>();

        if (_player == null || playerCorruptionText == null || playerCorruptionRoundsText == null) return;

        playerCorruptionText.text = _player.playerCorruption + " / " + _player.playerMaxCorruption;
        playerCorruptionRoundsText.text = _player.corruptionDebuffTurns.ToString();
    }
}
