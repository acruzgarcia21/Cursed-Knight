using CursedKnight;
using UnityEngine;

public class Player : MonoBehaviour
{
    // =========================================================
    // PLAYER STATS
    // =========================================================

    public int playerHealth;
    public int playerMaxHealth = 100;

    public int playerEnergy        = 3;
    public int playerEnergyPerTurn = 3;

    public int playerBlock;

    public int playerCorruption;
    public int playerMaxCorruption = 10;
    public int corruptionDamage    = 10;

    public int nextAttackEnergyReduction;

    public bool endlessAssaultTriggeredThisTurn;

    // =========================================================
    // REFERENCES
    // =========================================================

    private PlayerDisplay _playerDisplay;
    private UIDisplay _uiDisplay;
    private StatusManager _statusManager;
    private EnemyManager _enemyManager;
    private CombatFeedbackManager _combatFeedbackManager;

    private void Awake()
    {
        _playerDisplay         = GetComponent<PlayerDisplay>();
        _statusManager         = GetComponent<StatusManager>();
        _combatFeedbackManager = GetComponent<CombatFeedbackManager>();

        _uiDisplay   = FindFirstObjectByType<UIDisplay>();
        _enemyManager = FindFirstObjectByType<EnemyManager>();

        _playerDisplay.UpdatePlayerDisplay();
    }


    // =========================================================
    // BATTLE / TURN LIFECYCLE
    // =========================================================

    public void BattleSetup()
    {
        ClearNextAttackEnergyReduction();
        ResetEndlessAssaultTrigger();

        playerHealth     = playerMaxHealth;
        playerEnergy     = playerEnergyPerTurn;
        playerBlock      = 0;
        playerCorruption = 0;
    }

    public void StartTurn()
    {
        ClearBlock();
        ResetEnergy();
        ProcessStartTurnEffects();
        ResetEndlessAssaultTrigger();

        _uiDisplay.UpdatePlayerEnergyText(this);
        _uiDisplay.UpdatePlayerCorruptionText(this);
    }

    public void EndTurn()
    {
        ProcessEndTurnStatuses();
        _statusManager.TickDurations();
        _enemyManager.RefreshEnemyDisplays();
    }


    // =========================================================
    // HEALTH
    // =========================================================

    public void TakeDamage(int damage)
    {
        var initialHp = playerHealth;
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

        var healthLost = initialHp - playerHealth;
        var blockLost = blockBefore - playerBlock;

        _combatFeedbackManager.ShowDamageNumber(healthLost);

        Debug.Log(
            $"Player Damage | Raw: {damage} | " +
            $"HP Lost: {healthLost} | Block Lost: {blockLost} | " +
            $"Health: {initialHp} -> {playerHealth}"
        );

        if (PlayerIsDead())
        {
            BattleManager.Instance.LoseBattle();
        }

        _playerDisplay.UpdatePlayerDisplay();
    }

    public void LoseHealth(int healthToLose)
    {
        var initialHp = playerHealth;

        playerHealth = Mathf.Max(0, playerHealth - healthToLose);

        var healthLost = initialHp - playerHealth;
        _combatFeedbackManager.ShowDamageNumber(healthLost);

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

    private bool PlayerIsDead()
    {
        return playerHealth == 0;
    }


    // =========================================================
    // BLOCK
    // =========================================================

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


    // =========================================================
    // ENERGY
    // =========================================================

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

    private void ResetEnergy()
    {
        playerEnergy = playerEnergyPerTurn;

        _uiDisplay.UpdatePlayerEnergyText(this);
    }


    // =========================================================
    // NEXT ATTACK ENERGY REDUCTION
    // =========================================================

    public void AddNextAttackEnergyReduction(int amount)
    {
        if (amount < 0) return;

        nextAttackEnergyReduction += amount;
    }

    public void ClearNextAttackEnergyReduction()
    {
        nextAttackEnergyReduction = 0;
    }


    // =========================================================
    // CORRUPTION
    // =========================================================

    public void GainCorruption(int corruption)
    {
        playerCorruption += corruption;

        _playerDisplay.UpdatePlayerDisplay();
        _uiDisplay.UpdatePlayerCorruptionText(this);

        if (playerCorruption < playerMaxCorruption) return;

        ProcessMaxCorruptionTriggeredEffects();
        TriggerCorruptionOverflow();

        if (HasStatus(StatusEffect.StatusType.Corruption))
        {
            _enemyManager.RefreshEnemyDisplays();
        }
    }

    private void TriggerCorruptionOverflow()
    {
        TakeDamage(corruptionDamage);

        playerCorruption = 0;

        var corruptedStatus = new StatusEffect
        {
            statusType = StatusEffect.StatusType.Corruption,
            amount = 1,
            duration = 2
        };

        ApplyStatus(corruptedStatus);

        _playerDisplay.UpdatePlayerDisplay();
        _uiDisplay.UpdatePlayerCorruptionText(this);
    }


    // =========================================================
    // DAMAGE MODIFIERS
    // =========================================================

    public int GetModifiedAttackDamage(int baseDamage)
    {
        var modifiedDamage = baseDamage;

        if (_statusManager.HasStatus(StatusEffect.StatusType.Strength))
        {
            var strength =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Strength);

            modifiedDamage += strength;
        }

        if (_statusManager.HasStatus(StatusEffect.StatusType.Weak))
        {
            var weak =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Weak);

            modifiedDamage -= weak;
        }

        if (_statusManager.HasStatus(StatusEffect.StatusType.CorruptedSoul))
        {
            var damageIncrease =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.CorruptedSoul);

            var corruptionScale = playerCorruption * damageIncrease;

            modifiedDamage += corruptionScale;
        }

        if (modifiedDamage < 0)
        {
            modifiedDamage = 0;
        }

        return modifiedDamage;
    }

    public int GetModifiedBleedDamage(int baseBleedDamage)
    {
        var modifiedBleedDamage = baseBleedDamage;

        if (!_statusManager.HasStatus(StatusEffect.StatusType.BloodMoon))
        {
            return modifiedBleedDamage;
        }

        modifiedBleedDamage =
            Mathf.FloorToInt(modifiedBleedDamage * 1.5f);

        return modifiedBleedDamage;
    }

    private int GetModifiedIncomingDamage(int baseDamage)
    {
        var modifiedDamage = baseDamage;

        if (_statusManager.HasStatus(StatusEffect.StatusType.Vulnerable))
        {
            modifiedDamage =
                Mathf.FloorToInt(modifiedDamage * 1.5f);
        }

        if (_statusManager.HasStatus(StatusEffect.StatusType.Corruption))
        {
            modifiedDamage =
                Mathf.FloorToInt(modifiedDamage * 1.25f);
        }

        return modifiedDamage;
    }


    // =========================================================
    // STATUS ACCESS
    // =========================================================

    public void ApplyStatus(StatusEffect statusEffect)
    {
        _statusManager.ApplyStatus(statusEffect);
        _statusManager.DebugPrintStatuses();
    }

    public int GetStatusDuration(StatusEffect.StatusType statusType)
    {
        return _statusManager.GetStatusDuration(statusType);
    }

    public bool HasStatus(StatusEffect.StatusType statusType)
    {
        return _statusManager.HasStatus(statusType);
    }

    public int GetStatusAmount(StatusEffect.StatusType statusType)
    {
        return _statusManager.GetStatusAmount(statusType);
    }

    public StatusEffect GetStatus(StatusEffect.StatusType statusType)
    {
        return _statusManager.GetStatus(statusType);
    }


    // =========================================================
    // STATUS PROCESSING
    // =========================================================

    private void ProcessStartTurnEffects()
    {
        if (_statusManager.HasStatus(StatusEffect.StatusType.DarkMomentum))
        {
            var energyToGain =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.DarkMomentum);

            GainEnergy(energyToGain);
        }
    }

    private void ProcessEndTurnStatuses()
    {
        if (_statusManager.HasStatus(StatusEffect.StatusType.Poison))
        {
            var healthBefore = playerHealth;

            var statusAmount =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Poison);

            playerHealth = Mathf.Max(0, playerHealth - statusAmount);

            var healthLost = healthBefore - playerHealth;
            _combatFeedbackManager.ShowDamageNumber(healthLost);

            Debug.Log(
                $"Player Poison | Damage: {healthLost} | " +
                $"Health: {healthBefore} -> {playerHealth}"
            );
        }

        if (PlayerIsDead())
        {
            BattleManager.Instance.LoseBattle();
        }

        _playerDisplay.UpdatePlayerDisplay();
    }

    public void ProcessOnActionStatuses()
    {
        if (_statusManager.HasStatus(StatusEffect.StatusType.Bleed))
        {
            var healthBefore = playerHealth;

            var statusAmount =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Bleed);

            playerHealth = Mathf.Max(0, playerHealth - statusAmount);

            var healthLost = healthBefore - playerHealth;
            _combatFeedbackManager.ShowDamageNumber(healthLost);
        }

        if (_statusManager.HasStatus(StatusEffect.StatusType.Corruption))
        {
            var healthBefore = playerHealth;

            var statusAmount =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.Corruption);

            playerHealth = Mathf.Max(0, playerHealth - statusAmount);

            var healthLost = healthBefore - playerHealth;
            _combatFeedbackManager.ShowDamageNumber(healthLost);
        }

        if (PlayerIsDead())
        {
            BattleManager.Instance.LoseBattle();
        }

        _playerDisplay.UpdatePlayerDisplay();
    }


    // =========================================================
    // POWER / TRIGGERED EFFECTS
    // =========================================================

    public void ProcessCardTypeTriggeredEffects(Card.CardType cardType)
    {
        if (cardType != Card.CardType.Attack) return;

        if (_statusManager.HasStatus(StatusEffect.StatusType.ViciousResolve))
        {
            var blockToGain =
                _statusManager.GetStatusAmount(StatusEffect.StatusType.ViciousResolve);

            GainBlock(blockToGain);
        }
    }

    public void ProcessBleedAppliedTriggerEffects(Enemy enemy)
    {
        if (enemy == null) return;
        if (!_statusManager.HasStatus(StatusEffect.StatusType.BloodCurse)) return;

        var bleedBonusDamage = _statusManager.GetStatusAmount(StatusEffect.StatusType.BloodCurse);
        
        if (bleedBonusDamage < 0) return;
        enemy.LoseHealth(bleedBonusDamage);
    }

    private void ProcessMaxCorruptionTriggeredEffects()
    {
        var statusEffect =
            _statusManager.GetStatus(StatusEffect.StatusType.DarkCommunion);

        if (statusEffect == null) return;
        if (statusEffect.hasTriggered) return;
        if (statusEffect.statusToCreate == null) return;

        var statusToCreate = new StatusEffect
        {
            statusType = statusEffect.statusToCreate.statusType,
            amount     = statusEffect.statusToCreate.amount,
            duration   = statusEffect.statusToCreate.duration
        };

        ApplyStatus(statusToCreate);

        statusEffect.hasTriggered = true;
    }

    private void ResetEndlessAssaultTrigger()
    {
        endlessAssaultTriggeredThisTurn = false;
    }

    public void TriggerEndlessAssault()
    {
        endlessAssaultTriggeredThisTurn = true;
    }
}