using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int currentEnemyHealth;
    public int currentEnemyBlock;

    public EnemyData enemyData;

    private EnemyDisplay _enemyDisplay;
    private StatusManager _statusManager;

    private void Awake()
    {
        _enemyDisplay  = GetComponent<EnemyDisplay>();
        _statusManager = GetComponent<StatusManager>();
    }

    public void BattleSetup()
    {
        currentEnemyHealth = enemyData.enemyMaxHealth;
        currentEnemyBlock = 0;

        _enemyDisplay.UpdateEnemyDisplay();
    }

    public void TakeTurn(Player player)
    {
        if (enemyData.appliesStatus)
        {
            var statusEffect = new StatusEffect
            {
                statusType = enemyData.statusType,
                amount = enemyData.statusAmount,
                duration = enemyData.statusDuration
            }; 
            
            player.ApplyStatus(statusEffect);
        }

        var enemyAttackDamage = GetModifiedAttackDamage(enemyData.enemyAttackDamage);
        
        player.TakeDamage(enemyAttackDamage);
        
        _statusManager.TickDurations();
    }

    public void TakeDamage(int damage)
    {
        if (currentEnemyBlock > 0)
        {
            if (currentEnemyBlock >= damage)
            {
                currentEnemyBlock -= damage;
            }
            else
            {
                damage -= currentEnemyBlock;
                currentEnemyBlock = 0;
                currentEnemyHealth -= damage;
            }
        }
        else
        {
            currentEnemyHealth -= damage;
        }

        currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, enemyData.enemyMaxHealth);

        _enemyDisplay.UpdateEnemyDisplay();

        if (EnemyIsDead())
        {
            BattleManager.Instance.EnemyManager.RemoveEnemy(this);
        }
    }
    
    public void ApplyStatus(StatusEffect statusEffect)
    {
        _statusManager.ApplyStatus(statusEffect);
        _statusManager.DebugPrintStatuses();
    }
    
    private int GetModifiedAttackDamage(int baseDamage)
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

    private bool EnemyIsDead()
    {
        return currentEnemyHealth <= 0;
    }
    
    
}