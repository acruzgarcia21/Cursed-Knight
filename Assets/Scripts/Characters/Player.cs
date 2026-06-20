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

    private void Awake()
    {
        BattleSetup();
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
        playerEnergy = playerMaxEnergy;
    }

    public void EndTurn()
    {
        ClearBlock();
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
    }
    
    public void GainEnergy(int energy)
    {
        playerEnergy += energy;
    }

    public void ResetEnergy()
    {
        playerEnergy = 0;
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
    }

    public void Heal(int heal)
    {
        playerHealth += heal;
        playerHealth = Mathf.Clamp(playerHealth, 0, playerMaxHealth);
    }
    
    public void GainBlock(int block)
    {
        playerBlock += block;
    }

    private void ClearBlock()
    {
        playerBlock = 0;
    }

    public void GainCorruption(int corruption)
    {
        playerCorruption += corruption;

        if (playerCorruption < playerMaxCorruption) return;
        isCorrupted = true;
        TriggerCorruptionOverflow();
    }

    private void TriggerCorruptionOverflow()
    {
        TakeDamage(corruptionDamage);
        playerCorruption      = 0;
        corruptionDebuffTurns = 2;
    }

    public bool PlayerIsDead()
    {
        return playerHealth == 0;
    }
}
