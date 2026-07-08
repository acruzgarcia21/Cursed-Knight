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
        
        _uiDisplay     = FindFirstObjectByType<UIDisplay>();
        _statusManager = FindFirstObjectByType<StatusManager>();
        
        _playerDisplay.UpdatePlayerDisplay();
    }
    
    private void Start()
    {
        var weakOne = new StatusEffect
        {
            statusType = StatusEffect.StatusType.Weak,
            amount = 1,
            duration = 2
        };

        var weakTwo = new StatusEffect
        {
            statusType = StatusEffect.StatusType.Weak,
            amount = 2,
            duration = 1
        };

        _statusManager.ApplyStatus(weakOne);
        _statusManager.ApplyStatus(weakTwo);

        _statusManager.DebugPrintStatuses();
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

    public void ResetEnergy()
    {
        playerEnergy = playerEnergyPerTurn;
        
        _uiDisplay.UpdatePlayerEnergyText(this);
    }

    public void TakeDamage(int damage)
    {
        if (playerBlock > 0)
        {
            if (playerBlock >= damage)
            {
                playerBlock -= damage;
            }
            else
            {
                damage -= playerBlock;
                playerBlock = 0;
                playerHealth -= damage;
            }
        }
        else
        {
            playerHealth -= damage;   
        }
        playerHealth = Mathf.Clamp(playerHealth, 0, playerMaxHealth);

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

    private void ClearBlock()
    {
        playerBlock = 0;
        
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

    private void TriggerCorruptionOverflow()
    {
        TakeDamage(corruptionDamage);
        
        playerCorruption      = 0;
        corruptionDebuffTurns = 2;
        
        _playerDisplay.UpdatePlayerDisplay();
        _uiDisplay.UpdatePlayerCorruptionText(this);
    }

    public bool PlayerIsDead()
    {
        return playerHealth == 0;
    }
}
