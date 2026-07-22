using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int currentEnemyHealth;
    public int currentEnemyBlock;

    public bool isHidden;

    public EnemyData enemyData;

    private EnemyDisplay _enemyDisplay;
    private StatusManager _statusManager;

    private EnemyActionData _currentAction;
    private int _currentActionIndex;
    private int _currentActionConsecutiveUses;

    // Allows other systems to read current enemy action
    public EnemyActionData CurrentAction => _currentAction;

    private void Awake()
    {
        _enemyDisplay = GetComponent<EnemyDisplay>();
        _statusManager = GetComponent<StatusManager>();
    }

    public void BattleSetup()
    {
        currentEnemyHealth = enemyData.enemyMaxHealth;
        currentEnemyBlock  = 0;
        isHidden           = false;

        _enemyDisplay.UpdateEnemyDisplay();
    }

    public void TakeTurn(Player player)
    {
        if (player == null || _currentAction == null || player.playerHealth <= 0)
        {
            return;
        }

        currentEnemyBlock = 0;
        isHidden = _currentAction.hidesEnemy;

        if (_currentAction.damage > 0)
        {
            var hitCount = Mathf.Max(1, _currentAction.hitCount);
            var modifiedDamage = GetCurrentIntentDamage();

            for (var i = 0; i < hitCount; i++)
            {
                if (player.playerHealth <= 0) return;
                player.TakeDamage(modifiedDamage);
            }

            Debug.Log(
                $"{enemyData.enemyName} uses {_currentAction.actionName} " +
                $"for {modifiedDamage} damage x{hitCount}."
            );
        }

        if (_currentAction.blockAmount > 0)
        {
            GainBlock(_currentAction.blockAmount);
        }

        if (_currentAction.healingAmount > 0 && currentEnemyHealth != enemyData.enemyMaxHealth)
        {
            Heal(_currentAction.healingAmount);
        }

        ApplyCurrentActionStatus(player);

        ProcessOnActionStatuses();
        if (EnemyIsDead()) return;

        ProcessEndTurnStatuses();
        if (EnemyIsDead()) return;

        _statusManager.TickDurations();
    }

    // Chooses enemy's first action
    public void InitializeIntent()
    {
        if (enemyData == null || enemyData.enemyActions == null || enemyData.enemyActions.Count == 0)
        {
            _currentAction = null;
            return;
        }

        _currentActionIndex = Random.Range(0, enemyData.enemyActions.Count);
        _currentAction = enemyData.enemyActions[_currentActionIndex];
        _currentActionConsecutiveUses = 1;

        _enemyDisplay.UpdateEnemyDisplay();
    }

    public int GetCurrentIntentDamage()
    {
        if (_currentAction == null || _currentAction.damage <= 0)
        {
            return 0;
        }

        return GetModifiedAttackDamage(_currentAction.damage);
    }

    public void SelectNextAction()
    {
        if (enemyData == null || enemyData.enemyActions == null || enemyData.enemyActions.Count == 0)
        {
            _currentAction = null;
            return;
        }


        switch (enemyData.actionSelectionType)
        {
            case EnemyData.ActionSelectionType.FixedPattern:
                SelectFixedPatternAction();
                break;

            case EnemyData.ActionSelectionType.WeightedRandom:
                SelectWeightedRandomAction();
                break;
        }
    }

    private void SelectFixedPatternAction()
    {
        _currentActionIndex++;

        if (_currentActionIndex >= enemyData.enemyActions.Count)
        {
            _currentActionIndex = 0;
        }

        _currentAction = enemyData.enemyActions[_currentActionIndex];
        _enemyDisplay.UpdateEnemyDisplay();
    }

    private void SelectWeightedRandomAction()
    {
        var allowedActions = new List<EnemyActionData>();
        var oldCurrentAction = _currentAction;

        foreach (var action in enemyData.enemyActions)
        {
            if (action.selectionWeight <= 0) continue;
            if (action == _currentAction && _currentActionConsecutiveUses >= action.maximumConsecutiveUses)
                continue;

            allowedActions.Add(action);
        }

        // Fallback if every action was filtered out
        if (allowedActions.Count == 0)
        {
            foreach (var action in enemyData.enemyActions)
            {
                if (action.selectionWeight <= 0) continue;

                allowedActions.Add(action);
            }
        }

        // Calculate the total weight of the pool
        var totalWeight = 0;

        foreach (var action in allowedActions)
        {
            totalWeight += action.selectionWeight;
        }

        var roll = Random.Range(0, totalWeight);
        var runningWeight = 0;

        // Roll somewhere inside the total weight
        foreach (var action in allowedActions)
        {
            runningWeight += action.selectionWeight;

            if (roll >= runningWeight) continue;
            _currentAction = action;

            if (_currentAction == oldCurrentAction) _currentActionConsecutiveUses++;
            if (_currentAction != oldCurrentAction) _currentActionConsecutiveUses = 1;
            _enemyDisplay.UpdateEnemyDisplay();
            return;
        }
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
    
    private void ApplyCurrentActionStatus(Player player)
    {
        if (!_currentAction.appliesStatus || _currentAction.statusAmount <= 0)
        {
            return;
        }

        var statusEffect = new StatusEffect
        {
            statusType = _currentAction.statusType,
            amount = _currentAction.statusAmount,
            duration = _currentAction.statusDuration
        };

        switch (_currentAction.statusTarget)
        {
            case EnemyActionData.StatusTargetType.Self:
                ApplyStatus(statusEffect);
                break;

            case EnemyActionData.StatusTargetType.Player:
                player.ApplyStatus(statusEffect);
                break;

            case EnemyActionData.StatusTargetType.AllOtherAllies:
            case EnemyActionData.StatusTargetType.RandomAlly:
                Debug.LogWarning(
                    $"{_currentAction.statusTarget} is not implemented yet."
                );
                break;
        }
    }
    
    private void GainBlock(int block)
    {
        currentEnemyBlock += block;
        
        _enemyDisplay.UpdateEnemyDisplay();
    }
    
    private void Heal(int heal)
    {
        currentEnemyHealth += heal;
        currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, enemyData.enemyMaxHealth);
        
        _enemyDisplay.UpdateEnemyDisplay();
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