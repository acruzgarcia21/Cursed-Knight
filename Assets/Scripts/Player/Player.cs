using System;
using UnityEngine;
public class Player : MonoBehaviour
{
    public int playerHealth;
    public int playerMaxHealth = 100;
    
    public int playerEnergy = 3;
    public int playerEnergyPerTurn = 3;
    
    public int playerBlock;
    
    public int playerCorruption;
    public int playerMaxCorruption = 10;

    public int corruptionDebuffTurns;
    public bool isCorrupted;
    public int corruptionDamage = 10;
    
    private PlayerDisplay _playerDisplay;
    private UIDisplay _uiDisplay;
    private StatusManager _statusManager;
    
    private void Awake()
    {
        _playerDisplay = GetComponent<PlayerDisplay>();
        _statusManager = GetComponent<StatusManager>();
        
        _uiDisplay     = FindFirstObjectByType<UIDisplay>();
        
        _playerDisplay.UpdatePlayerDisplay();
    }

    public void BattleSetup()
    {
        playerHealth = playerMaxHealth;
        playerEnergy = playerEnergyPerTurn;
        playerBlock = 0;
        playerCorruption = 0;
    }

    public void StartTurn()
    {
        ClearBlock(); 
        ResetEnergy();
        _uiDisplay.UpdatePlayerEnergyText(this);
        _uiDisplay.UpdatePlayerCorruptionText(this);
    }

    public void EndTurn()
    {
        if (isCorrupted && corruptionDebuffTurns > 0)
        {
            corruptionDebuffTurns--;
        }
        corruptionDebuffTurns = Mathf.Clamp(corruptionDebuffTurns, 0, 2);
        
        if (corruptionDebuffTurns <= 0) isCorrupted = false;
        
        ProcessEndTurnStatuses();
        
        _statusManager.TickDurations();
    }

    public void SpendEnergy(int amount)
    {
        playerEnergy -= amount;
        playerEnergy = Mathf.Max(playerEnergy, 0);
        
        _playerDisplay.UpdatePlayerDisplay();
        _uiDisplay.UpdatePlayerEnergyText(this);
    }
    
    public void GainEnergy(int energy)
    {
        playerEnergy += energy;
        _playerDisplay.UpdatePlayerDisplay();
        
        _uiDisplay.UpdatePlayerEnergyText(this);
    }

    public void TakeDamage(int damage)
    {
        var healthBefore = playerHealth;
        var blockBefore = playerBlock;
        var modifiedDamage = GetModifiedIncomingDamage(damage);

        if (playerBlock > 0)
        {
            if (playerBlock >= modifiedDamage)
            {
                playerBlock -= modifiedDamage;
            }
            else
            {
                modifiedDamage -= playerBlock;
                playerBlock = 0;
                playerHealth -= modifiedDamage;
            }
        }
        else
        {
            playerHealth -= modifiedDamage;
        }

        playerHealth = Mathf.Clamp(playerHealth, 0, playerMaxHealth);

        var healthLost = healthBefore - playerHealth;
        var blockLost = blockBefore - playerBlock;

        Debug.Log(
            $"Player Damage | Raw: {damage} | " +
            $"HP Lost: {healthLost} | Block Lost: {blockLost} | " +
            $"Health: {healthBefore} -> {playerHealth}"
        );

        if (PlayerIsDead())
        {
            BattleManager.Instance.LoseBattle();
        }

        _playerDisplay.UpdatePlayerDisplay();
    }

    public void Heal(int heal)
    {
        playerHealth += heal;
        playerHealth = Mathf.Clamp(playerHealth, 0, playerMaxHealth);
        
        _playerDisplay.UpdatePlayerDisplay();
    }
    
    public void GainBlock(int block)
    {
        playerBlock += block;
        
        _playerDisplay.UpdatePlayerDisplay();
    }

    public void GainCorruption(int corruption)
    {
        playerCorruption += corruption;
        
        _playerDisplay.UpdatePlayerDisplay();
        _uiDisplay.UpdatePlayerCorruptionText(this);

        if (playerCorruption < playerMaxCorruption) return;
        
        isCorrupted = true;
        
        TriggerCorruptionOverflow();
    }
    
    public int GetModifiedAttackDamage(int baseDamage)
    {
        var modifiedDamage = baseDamage;
        if (_statusManager.HasStatus(StatusEffect.StatusType.Strength))
        {
            var strength = _statusManager.GetStatusAmount(StatusEffect.StatusType.Strength);
            modifiedDamage += strength;
        }

        if (_statusManager.HasStatus(StatusEffect.StatusType.Weak))
        {
            var weak = _statusManager.GetStatusAmount(StatusEffect.StatusType.Weak);
            modifiedDamage -= weak;
        }

        if (modifiedDamage < 0) modifiedDamage = 0;
        
        return modifiedDamage;
    }

    public void ApplyStatus(StatusEffect statusEffect)
    {
        _statusManager.ApplyStatus(statusEffect);
        _statusManager.DebugPrintStatuses();
    }
    
    private int GetModifiedIncomingDamage(int baseDamage)
    {
        var modifiedDamage = baseDamage;

        if (_statusManager.HasStatus(StatusEffect.StatusType.Vulnerable))
        {
            modifiedDamage = Mathf.FloorToInt(modifiedDamage * 1.5f);
        }
        
        return modifiedDamage;
    }
    
    private void ProcessEndTurnStatuses()
    {
        if (_statusManager.HasStatus(StatusEffect.StatusType.Poison))
        {
            var healthBefore = playerHealth;
            var poisonDamage =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Poison);

            playerHealth -= poisonDamage;
            playerHealth = Mathf.Clamp(playerHealth, 0, playerMaxHealth);

            Debug.Log(
                $"Player Poison | Damage: {healthBefore - playerHealth} | " +
                $"Health: {healthBefore} -> {playerHealth}"
            );
        }

        if (PlayerIsDead())
        {
            BattleManager.Instance.LoseBattle();
        }

        _playerDisplay.UpdatePlayerDisplay();
    }
    
    private void ClearBlock()
    {
        playerBlock = 0;
        
        _playerDisplay.UpdatePlayerDisplay();
    }
    
    private void ResetEnergy()
    {
        playerEnergy = playerEnergyPerTurn;
        
        _uiDisplay.UpdatePlayerEnergyText(this);
    }

    private void TriggerCorruptionOverflow()
    {
        TakeDamage(corruptionDamage);
        
        playerCorruption      = 0;
        corruptionDebuffTurns = 2;
        
        _playerDisplay.UpdatePlayerDisplay();
        _uiDisplay.UpdatePlayerCorruptionText(this);
    }

    private bool PlayerIsDead()
    {
        return playerHealth == 0;
    }
}
