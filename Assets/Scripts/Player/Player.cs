using UnityEngine;
public class Player : MonoBehaviour
{
    public int playerHealth;
    public int playerMaxHealth = 100;
    
    public int playerEnergy = 3;
    public int playerMaxEnergy = 3;
    
    public int playerBlock;
    
    public int playerCorruption;
    public int playerMaxCorruption = 10;

    public int corruptionDebuffTurns;
    public bool isCorrupted = false;
    public int corruptionDamage = 10;
    
    private PlayerDisplay _playerDisplay;
    private UIManager _uiManager;

    private void Awake()
    {
        _playerDisplay = GetComponent<PlayerDisplay>();
        _uiManager = FindFirstObjectByType<UIManager>();

        BattleSetup();

        _playerDisplay.UpdatePlayerDisplay();
    }

    private void BattleSetup()
    {
        playerHealth = playerMaxHealth;
        playerEnergy = playerMaxEnergy;
        playerBlock = 0;
        playerCorruption = 0;
    }

    public void StartTurn()
    {
        ClearBlock(); 
        ResetEnergy();
        _uiManager.UpdatePlayerCorruptionText();
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
        _playerDisplay.UpdatePlayerDisplay();
        _uiManager.UpdatePlayerEnergyText();
    }
    
    public void GainEnergy(int energy)
    {
        playerEnergy += energy;
        _playerDisplay.UpdatePlayerDisplay();
        _uiManager.UpdatePlayerEnergyText();
    }

    public void ResetEnergy()
    {
        playerEnergy = playerMaxEnergy;
        _uiManager.UpdatePlayerEnergyText();
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
        _uiManager.UpdatePlayerCorruptionText();

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
        _uiManager.UpdatePlayerCorruptionText();
    }

    public bool PlayerIsDead()
    {
        return playerHealth == 0;
    }
}
