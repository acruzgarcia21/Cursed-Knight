using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int currentEnemyHealth;
    public int currentEnemyBlock;

    public EnemyData enemyData;

    private EnemyDisplay _enemyDisplay;
    private StatusManager _statusManager;
    
    private EnemyActionData _currentAction;

    private int _currentActionIndex;

    public EnemyActionData CurrentAction => _currentAction;
    
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
        if (player == null || _currentAction == null)
        {
            return;
        }

        if (_currentAction.damage > 0)
        {
            int hitCount = Mathf.Max(1, _currentAction.hitCount);
            int modifiedDamage = GetCurrentIntentDamage();

            for (int i = 0; i < hitCount; i++)
            {
                player.TakeDamage(modifiedDamage);
            }

            Debug.Log(
                $"{enemyData.enemyName} uses {_currentAction.actionName} " +
                $"for {modifiedDamage} damage x{hitCount}."
            );
        }

        ProcessOnActionStatuses();
        ProcessEndTurnStatuses();

        _statusManager.TickDurations();
    }
    
    public void InitializeIntent()
    {
        if (enemyData == null || enemyData.enemyActions.Count == 0)
        {
            _currentAction = null;
            return;
        }

        _currentActionIndex = Random.Range(0, enemyData.enemyActions.Count);
        _currentAction = enemyData.enemyActions[_currentActionIndex];
    }

    public int GetCurrentIntentDamage()
    {
        if (_currentAction == null || _currentAction.damage <= 0)
        {
            return 0;
        }

        return GetModifiedAttackDamage(_currentAction.damage);
    }
    
    public void SelectNextFixedAction()
    {
        if (enemyData == null || enemyData.enemyActions.Count == 0)
        {
            _currentAction = null;
            return;
        }

        _currentActionIndex++;

        if (_currentActionIndex >= enemyData.enemyActions.Count)
        {
            _currentActionIndex = 0;
        }

        _currentAction = enemyData.enemyActions[_currentActionIndex];
    }

    public void TakeDamage(int damage)
    {
        var modifiedDamage = GetModifiedIncomingDamage(damage);
        
        if (currentEnemyBlock > 0)
        {
            if (currentEnemyBlock >= modifiedDamage)
            {
                currentEnemyBlock -= modifiedDamage;
            }
            else
            {
                modifiedDamage -= currentEnemyBlock;
                currentEnemyBlock = 0;
                currentEnemyHealth -= modifiedDamage;
            }
        }
        else
        {
            currentEnemyHealth -= modifiedDamage;
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
            var poisonDamage = 
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Poison);
            
            currentEnemyHealth -= poisonDamage;
        }
        
        currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, enemyData.enemyMaxHealth);

        _enemyDisplay.UpdateEnemyDisplay();

        if (EnemyIsDead())
        {
            BattleManager.Instance.EnemyManager.RemoveEnemy(this);
        }
    }
    
    private void ProcessOnActionStatuses()
    {
        if (_statusManager.HasStatus(StatusEffect.StatusType.Bleed))
        {
            var bleedAmount = 
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Bleed);
            
            currentEnemyHealth -= bleedAmount;
            currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, enemyData.enemyMaxHealth);

            Debug.Log(
                $"Player Bleed | Damage: {currentEnemyHealth - bleedAmount} | " +
                $"Health: {bleedAmount} -> {currentEnemyHealth}"
            );
        }
        
        _enemyDisplay.UpdateEnemyDisplay();

        if (EnemyIsDead())
        {
            BattleManager.Instance.EnemyManager.RemoveEnemy(this);
        }
    }

    private bool EnemyIsDead()
    {
        return currentEnemyHealth <= 0;
    }
    
    
}