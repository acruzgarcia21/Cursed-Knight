using System.Collections;
using System.Collections.Generic;
using CursedKnight;
using UnityEngine;
using UnityEngine.Rendering;

public class CardPlayManager : MonoBehaviour
{
    public event System.Action OnAttackThresholdHit;
    
    private HandManager _handManager;
    private DiscardManager _discardManager;
    private EnemyManager _enemyManager;
    private ExhaustManager _exhaustManager;
    private DeckManager _deckManager;
    private AudioManager _audioManager;
    private TurnManager _turnManager;

    private int attacksPlayed;

    private bool consumesRelicDamage;
    private bool _isResolvingCard;

    public bool IsResolvingCard()
    {
        return _isResolvingCard;
    }

    private enum PostPlayDestination
    {
        Discard,
        OutOfCombat,
        Exhaust,
    }

    private void Awake()
    {
        _handManager    = FindAnyObjectByType<HandManager>();
        _discardManager = FindAnyObjectByType<DiscardManager>();
        _enemyManager   = FindAnyObjectByType<EnemyManager>();
        _exhaustManager = FindAnyObjectByType<ExhaustManager>();
        _deckManager    = FindAnyObjectByType<DeckManager>();
        _audioManager   = FindAnyObjectByType<AudioManager>();
        _turnManager    = FindAnyObjectByType<TurnManager>();
    }

    public bool TryPlayCard(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy)
    {
        if (!_turnManager.IsCombatActive() || _isResolvingCard) return false;
        
        if (_turnManager.GetTurnState() != TurnManager.TurnState.Player || _turnManager.IsResolvingTurn())
        {
            return false;
        }
        
        if (player == null || runtimeCard == null || runtimeCard.cardData == null)
        {
            return false;
        }

        var cardData = runtimeCard.cardData;
        var cardEnergyCost = cardData.GetCardEnergyCost(runtimeCard.isUpgraded);
        var finalEnergyCardCost = CalculateFinalCardEnergyCost(cardEnergyCost, player, cardData.cardType);

        if (player.playerEnergy < finalEnergyCardCost)
        {
            Debug.Log("Not enough energy!");
            return false;
        }

        if (!IsTargetValid(player, cardData, targetEnemy))
        {
            Debug.Log("Invalid Target!");
            return false;
        }

        return cardData.cardType switch
        {
            Card.CardType.Attack  => TryPlayAttack(player, runtimeCard, cardObject, targetEnemy, finalEnergyCardCost),
            Card.CardType.Defense => TryPlayDefense(player, runtimeCard, cardObject, targetEnemy, finalEnergyCardCost),
            Card.CardType.Utility => TryPlayUtility(player, runtimeCard, cardObject, targetEnemy, finalEnergyCardCost),
            Card.CardType.Power   => TryPlayPower(player, runtimeCard, cardObject, finalEnergyCardCost),
            _ => false
        };
    }

    private bool TryPlayAttack(Player player, RuntimeCard runtimeCard, 
        GameObject cardObject, Enemy targetEnemy, int cardEnergyCost)
    {
        var attackCard = runtimeCard.cardData as Attack;
        if (attackCard == null) return false;

        var hitCount = attackCard.GetHitCount(runtimeCard.isUpgraded);

        var scaledDamage = CalculateScaledAttackDamage(player, runtimeCard);

        var finalAttackDamage = player.GetModifiedAttackDamage(scaledDamage);

        BeginCardPlay(player, runtimeCard, cardEnergyCost);
        
        player.ClearNextAttackEnergyReduction();

        if (player.GetStoredRelicDamage() > 0) consumesRelicDamage = true;
        player.ResetRelicDamage();

        Debug.Log(
            $"Played attack card: {attackCard.cardName}," +
            $" Base Damage: {attackCard.GetAttackDamage(runtimeCard.isUpgraded)}," +
            $" Modified Damage: {finalAttackDamage}" 
        );

        switch (attackCard.targetType)
        {
            case Card.TargetType.AllEnemies:
            {
                var allLivingEnemies = _enemyManager.GetLivingEnemies();

                foreach (var enemy in allLivingEnemies)
                {
                    if (enemy.isHidden) continue;

                    for (var i = 0; i < hitCount; i++)
                    {
                        enemy.TakeDamage(finalAttackDamage, true, player);
                    }
                }

                break;
            }

            case Card.TargetType.RandomEnemy:
            {
                for (var i = 0; i < hitCount; i++)
                {
                    var allLivingEnemies = _enemyManager.GetLivingEnemies();
                    var visibleEnemies = new List<Enemy>();

                    foreach (var enemy in allLivingEnemies)
                    {
                        if (!enemy.isHidden)
                        {
                            visibleEnemies.Add(enemy);
                        }
                    }

                    if (visibleEnemies.Count == 0) break;

                    var randomEnemyIndex = Random.Range(0, visibleEnemies.Count);

                    visibleEnemies[randomEnemyIndex].TakeDamage(finalAttackDamage, true, player);
                }

                break;
            }

            case Card.TargetType.SingleEnemy:
            default:
            {
                for (var i = 0; i < hitCount; i++)
                {
                    targetEnemy.TakeDamage(finalAttackDamage, true, player);
                }

                break;
            }
        }

        player.ProcessCardTypeTriggeredEffects(attackCard.cardType);
        ApplyCardStatus(player, runtimeCard, targetEnemy);
        ApplyCardAdditionalStatus(player, targetEnemy, attackCard);
        StartCoroutine(CompleteCardPlay(runtimeCard, cardObject, player));

        return true;
    }

    private bool TryPlayDefense(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy, int cardEnergyCost)
    {
        var defenseCard = runtimeCard.cardData as Defense;
        if (defenseCard == null) return false;

        BeginCardPlay(player, runtimeCard, cardEnergyCost);
        ApplyCardStatus(player, runtimeCard, targetEnemy);
        ApplyAdditionalStatusToAllEnemies(player, runtimeCard);

        var finalBlockToGain = CalculateFinalBlock(runtimeCard);

        player.GainBlock(finalBlockToGain);

        StartCoroutine(CompleteCardPlay(runtimeCard, cardObject, player));

        return true;
    }

    private bool TryPlayUtility(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy, int cardEnergyCost)
    {
        var utilityCard = runtimeCard.cardData as UtilityCard;
        if (utilityCard == null) return false;

        BeginCardPlay(player, runtimeCard, cardEnergyCost);
        ApplyCardStatus(player, runtimeCard, targetEnemy);
        ProcessNextCardEnergyReduction(runtimeCard, player);

        var cardEnergyGain = utilityCard.GetCardEnergyGain(runtimeCard.isUpgraded);
        var cardHealthGain = utilityCard.GetCardHealthGain(runtimeCard.isUpgraded);

        if (cardEnergyGain > 0)
        {
            player.GainEnergy(cardEnergyGain);
        }

        if (cardHealthGain > 0)
        {
            player.Heal(cardHealthGain);
        }

        StartCoroutine(CompleteCardPlay(runtimeCard, cardObject, player));

        return true;
    }

    private bool TryPlayPower(Player player, RuntimeCard runtimeCard, GameObject cardObject, int cardEnergyCost)
    {
        var powerCard = runtimeCard.cardData as Power;
        if (powerCard == null) return false;

        BeginCardPlay(player, runtimeCard, cardEnergyCost);
        ApplyCardStatus(player, runtimeCard, null);
        StartCoroutine(CompleteCardPlay(runtimeCard, cardObject, player));

        return true;
    }

    private IEnumerator CompleteCardPlay(RuntimeCard runtimeCard, GameObject cardObject, Player player)
    {
        _isResolvingCard = true;
        try
        {
            var cardData = runtimeCard.cardData;

            const int attackThreshold = 3;
        
            if (cardData.cardType == Card.CardType.Attack && !consumesRelicDamage) attacksPlayed++;
        
            if (attacksPlayed == attackThreshold)
            {
                OnAttackThresholdHit?.Invoke();
                attacksPlayed = 0;
            }

            consumesRelicDamage = false;

            ApplyCardHealthLoss(player, runtimeCard);
            DrawCardsFromCard(runtimeCard);
            ApplyRandomCardDiscard(runtimeCard);
            DrawRandomCardFromDiscard(runtimeCard);
            ApplyCardBonusEnergy(player, runtimeCard);

            yield return player.ProcessOnActionStatuses();

            var destination = DeterminePostPlayDestination(runtimeCard);

            switch (destination)
            {
                case PostPlayDestination.Discard:
                    SendCardToDiscard(runtimeCard, cardObject);
                    break;

                case PostPlayDestination.OutOfCombat:
                    RemoveCardFromCombat(cardObject);
                    break;

                case PostPlayDestination.Exhaust:
                    ExhaustCard(runtimeCard, cardObject, player);
                    break;
            }
        
            ResolveCardCreation(runtimeCard);
            yield return new WaitUntil(() => !_enemyManager.HasPendingDeaths());
        }
        finally
        {
            _isResolvingCard = false;
        }
    }

    private PostPlayDestination DeterminePostPlayDestination(RuntimeCard runtimeCard)
    {
        if (runtimeCard.cardData.cardType == Card.CardType.Power)
        {
            return PostPlayDestination.OutOfCombat;
        }
        if (runtimeCard.exhaust)
        {
            return PostPlayDestination.Exhaust;
        }
        
        return PostPlayDestination.Discard;
        
    }

    private void SendCardToDiscard(RuntimeCard runtimeCard, GameObject cardObject)
    {
        _handManager.RemoveCardFromHand(cardObject);
        _discardManager.AddToDiscardPile(runtimeCard);

        Destroy(cardObject);
    }

    private void RemoveCardFromCombat(GameObject cardObject)
    {
        _handManager.RemoveCardFromHand(cardObject);
        Destroy(cardObject);
    }

    private void ExhaustCard(RuntimeCard runtimeCard, GameObject cardObject, Player player)
    {
        _handManager.RemoveCardFromHand(cardObject);
        _exhaustManager.AddToExhaustPile(runtimeCard);
        
        ApplyCardBonusBlock(player);
        
        Destroy(cardObject);
    }

    private bool IsTargetValid(Player player, Card cardData, Enemy targetEnemy)
    {
        if (cardData == null) return false;

        switch (cardData.targetType)
        {
            case Card.TargetType.SingleEnemy:
                if (targetEnemy == null) return false;
                
                if (targetEnemy.isHidden)
                {
                    Debug.Log("Enemy is hidden for this turn, cannot target!");
                    return false;
                }

                return true;

            case Card.TargetType.AllEnemies:
            case Card.TargetType.RandomEnemy:

                var livingEnemies = _enemyManager.GetLivingEnemies();

                foreach (var enemy in livingEnemies)
                {
                    if (!enemy.isHidden) return true;
                }

                Debug.Log("There are no visible enemies to target");
                return false;

            case Card.TargetType.Self:
                return player != null;

            case Card.TargetType.None:
            default:
                return true;
        }
    }

    private void BeginCardPlay(Player player, RuntimeCard runtimeCard, int cardEnergyCost)
    {
        _audioManager.PlayCardPlaySound(runtimeCard.cardData.cardType);
        ApplyCardCorruption(player, runtimeCard);
        SpendCardEnergy(player, cardEnergyCost);
    }


    private void SpendCardEnergy(Player player, int cardEnergyCost)
    {
        if (cardEnergyCost > 0)
        {
            player.SpendEnergy(cardEnergyCost);
        }
    }

    private void ApplyCardCorruption(Player player, RuntimeCard runtimeCard)
    {
        var cardData = runtimeCard.cardData;
        var corruptionGain = cardData.GetCardCorruptionGain(runtimeCard.isUpgraded);

        if (corruptionGain > 0)
        {
            player.GainCorruption(corruptionGain);
        }
    }

    private int CalculateScaledAttackDamage(Player player, RuntimeCard runtimeCard)
    {
        var attackCard = runtimeCard.cardData as Attack;
        if (attackCard == null) return 0;
       
        var corruptionDamagePerPoint = attackCard.GetCorruptionDamagePerPoint(runtimeCard.isUpgraded);
        var baseDamage               = attackCard.GetAttackDamage(runtimeCard.isUpgraded);
        var scaledDamage             = baseDamage;
       
        if (attackCard.scalesWithCorruption && corruptionDamagePerPoint > 0)
        {
            var corruptionBonus = player.playerCorruption * corruptionDamagePerPoint;
            scaledDamage += corruptionBonus;
        }

        return scaledDamage;
    }
    
    private int CalculateFinalBlock(RuntimeCard runtimeCard)
    {
        var defenseCard = runtimeCard.cardData as Defense;
        if (defenseCard == null) return 0;
        
        
        var baseBlock   = defenseCard.GetCardBlock(runtimeCard.isUpgraded);
        var scaledBlock = baseBlock;
        var bonusBlock  = defenseCard.GetBonusBlockIfEnemyHasBleed(runtimeCard.isUpgraded);
        
        // Blood Guard
        if (_enemyManager.DoesAnyEnemyHaveStatus(StatusEffect.StatusType.Bleed) && bonusBlock > 0)
        {
            scaledBlock += bonusBlock;
        }

        return scaledBlock;
    }

    private int CalculateFinalCardEnergyCost(int cardEnergyCost, Player player, Card.CardType cardType)
    {
        var finalCardEnergyCost = cardEnergyCost;


        if (player.nextAttackEnergyReduction > 0 && cardType == Card.CardType.Attack)
        {
            finalCardEnergyCost -= player.nextAttackEnergyReduction;
            finalCardEnergyCost = Mathf.Max(finalCardEnergyCost, 0);
        }
        
        return finalCardEnergyCost;
    }

    private void ApplyCardHealthLoss(Player player, RuntimeCard runtimeCard)
    {
        var healthLoss = runtimeCard.cardData.GetCardHealthLoss(runtimeCard.isUpgraded);

        if (healthLoss > 0)
        {
            player.LoseHealth(healthLoss);
        }
    }

    private void DrawCardsFromCard(RuntimeCard runtimeCard)
    {
        var cardsToDraw = runtimeCard.cardData.GetCardsToDraw(runtimeCard.isUpgraded);

        if (cardsToDraw > 0)
        {
            _handManager.DrawCards(cardsToDraw);
        }
    }

    private void ApplyRandomCardDiscard(RuntimeCard runtimeCard)
    {
        var cardsToDiscard = runtimeCard.cardData.GetCardsToDiscardRandomly(runtimeCard.isUpgraded);

        if (cardsToDiscard > 0)
        {
            _handManager.DiscardRandomCards(cardsToDiscard);
        }
    }

    private void DrawRandomCardFromDiscard(RuntimeCard runtimeCard)
    {
        var cardsToDrawFromDiscard = runtimeCard.cardData.GetCardsToDrawFromDiscard(runtimeCard.isUpgraded);

        if (cardsToDrawFromDiscard <= 0) return;

        for (var i = 0; i < cardsToDrawFromDiscard; i++)
        {
            if (_handManager.IsHandFull()) break;

            var cardToDraw = _discardManager.PullRandomCardFromDiscard();

            if (cardToDraw == null) break;

            _handManager.AddCardToHand(cardToDraw);
        }
    }
    private void ApplyCardStatus(Player player, RuntimeCard runtimeCard, Enemy targetEnemy) 
    {
        var cardData = runtimeCard.cardData;

        if (!cardData.appliesStatus) return;

        var statusEffect = new StatusEffect
        {
            statusType = cardData.statusType,
            amount     = cardData.GetStatusAmount(runtimeCard.isUpgraded),
            duration   = cardData.GetStatusDuration(runtimeCard.isUpgraded)
        };

        if (cardData is Power powerCard && powerCard.statusToCreate != null)
        {
            statusEffect.statusToCreate = new StatusEffect
            {
                statusType = powerCard.statusToCreate.statusType,
                amount     = powerCard.statusToCreate.GetStatusAmount(runtimeCard.isUpgraded),
                duration   = powerCard.statusToCreate.GetStatusDuration(runtimeCard.isUpgraded)
            };
        }

        switch (cardData.statusTargetType)
        {
            case Card.StatusTargetType.Self:
            {
                player.ApplyStatus(statusEffect);
                break;
            }

            case Card.StatusTargetType.SingleEnemy:
            {
                if (targetEnemy == null) break;

                targetEnemy.ApplyStatus(statusEffect);

                if (statusEffect.statusType == StatusEffect.StatusType.Bleed)
                {
                    player.ProcessBleedAppliedTriggerEffects(targetEnemy);
                }

                break;
            }

            case Card.StatusTargetType.AllEnemies:
            {
                var livingEnemies = _enemyManager.GetLivingEnemies();

                foreach (var enemy in livingEnemies)
                {
                    if (enemy == null) continue;
                    if (enemy.isHidden) continue;

                    var statusToApply = new StatusEffect
                    {
                        statusType = statusEffect.statusType,
                        amount     = statusEffect.amount,
                        duration   = statusEffect.duration
                    };

                    enemy.ApplyStatus(statusToApply);

                    if (statusToApply.statusType == StatusEffect.StatusType.Bleed)
                    {
                        player.ProcessBleedAppliedTriggerEffects(enemy);
                    }
                }

                break;
            }

            case Card.StatusTargetType.RandomEnemy:
            {
                var livingEnemies = _enemyManager.GetLivingEnemies();
                var visibleEnemies = new List<Enemy>();

                foreach (var enemy in livingEnemies)
                {
                    if (enemy == null) continue;
                    if (enemy.isHidden) continue;

                    visibleEnemies.Add(enemy);
                }

                if (visibleEnemies.Count == 0) break;

                var randomEnemyIndex = Random.Range(0, visibleEnemies.Count);
                var randomEnemy = visibleEnemies[randomEnemyIndex];

                randomEnemy.ApplyStatus(statusEffect);

                if (statusEffect.statusType == StatusEffect.StatusType.Bleed)
                {
                    player.ProcessBleedAppliedTriggerEffects(randomEnemy);
                }

                break;
            }
        }
    }
    
    private void ApplyAdditionalStatusToAllEnemies(Player player, RuntimeCard runtimeCard)
    {
        var cardData = runtimeCard.cardData as Defense;
        if (cardData == null) return;
    
        if (!cardData.appliesStatusToAllEnemies) return;
        if (!cardData.appliesStatus) return;

        var livingEnemies = _enemyManager.GetLivingEnemies();

        foreach (var enemy in livingEnemies)
        {
            if (enemy == null) continue;
            if (enemy.isHidden) continue;

            var statusEffect = new StatusEffect
            {
                statusType = cardData.statusType,
                amount     = cardData.GetStatusAmount(runtimeCard.isUpgraded),
                duration   = cardData.GetStatusDuration(runtimeCard.isUpgraded)
            };

            enemy.ApplyStatus(statusEffect);

            if (statusEffect.statusType == StatusEffect.StatusType.Bleed)
            {
                player.ProcessBleedAppliedTriggerEffects(enemy);
            }
        }
    }

    private void ProcessNextCardEnergyReduction(RuntimeCard runtimeCard, Player player)
    {
        var cardData = runtimeCard.cardData;
    
        if (!cardData.reducesNextAttackEnergy) return;

        var nextEnergyReduction = cardData.GetEnergyToReduce(runtimeCard.isUpgraded);

        player.AddNextAttackEnergyReduction(nextEnergyReduction);
    }

    private void ResolveCardCreation(RuntimeCard runtimeCard)
    {
        var cardData = runtimeCard.cardData;

        if (!cardData.createsCards) return;

        var cardsToCreate = cardData.GetCardsToCreate(runtimeCard.isUpgraded);

        for (var i = 0; i < cardsToCreate; i++)
        {
            _deckManager.CreateCardDuringCombat(cardData.cardToCreate, cardData.createdCardDestination);
        }
    }

    private void ApplyCardBonusEnergy(Player player, RuntimeCard runtimeCard)
    {
        if (!player.HasStatus(StatusEffect.StatusType.EndlessAssault)) return;
        if (player.endlessAssaultTriggeredThisTurn)
        {
            Debug.Log("Already played a multihit attack, cannot gain more energy this turn!");
            return;
        }

        if (runtimeCard.cardData is Attack attackCard && attackCard.GetHitCount(runtimeCard.isUpgraded) > 1)
        {
            var energyToGain = player.GetStatusAmount(StatusEffect.StatusType.EndlessAssault);
            player.GainEnergy(energyToGain);
            player.TriggerEndlessAssault();
        }
    }

    private void ApplyCardBonusBlock(Player player)
    {
        if (!player.HasStatus(StatusEffect.StatusType.AshesOfWar)) return;

        var blockToGain = player.GetStatusAmount(StatusEffect.StatusType.AshesOfWar);
        player.GainBlock(blockToGain);
    }

    private void ApplyCardAdditionalStatus(Player player, Enemy enemy, Attack cardData)
    {
        if (!player.HasStatus(StatusEffect.StatusType.PatientHunter)) return;
        if (!cardData.retain) return;
        
        var statusEffect =
            player.GetStatus(StatusEffect.StatusType.PatientHunter);

        if (statusEffect == null) return;
        
        var statusToCreate = new StatusEffect
        {
            statusType = statusEffect.statusToCreate.statusType,
            amount     = statusEffect.statusToCreate.amount,
            duration   = statusEffect.statusToCreate.duration
        };
        
        if (statusToCreate.statusToCreate == null) return;
        
        enemy.ApplyStatus(statusToCreate);
    }
}