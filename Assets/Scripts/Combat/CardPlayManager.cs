using System.Collections.Generic;
using CursedKnight;
using UnityEngine;
using UnityEngine.Rendering;

public class CardPlayManager : MonoBehaviour
{
    private HandManager _handManager;
    private DiscardManager _discardManager;
    private EnemyManager _enemyManager;
    private ExhaustManager _exhaustManager;
    private DeckManager _deckManager;

    private enum PostPlayDestination
    {
        Discard,
        OutOfCombat,
        Exhaust,
    }

    private void Awake()
    {
        _handManager    = FindFirstObjectByType<HandManager>();
        _discardManager = FindFirstObjectByType<DiscardManager>();
        _enemyManager   = FindFirstObjectByType<EnemyManager>();
        _exhaustManager = FindFirstObjectByType<ExhaustManager>();
        _deckManager    = FindFirstObjectByType<DeckManager>();
    }

    public bool TryPlayCard(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy)
    {
        if (player == null || runtimeCard == null || runtimeCard.cardData == null)
        {
            return false;
        }

        var cardData = runtimeCard.cardData;
        var finalEnergyCardCost = CalculateFinalCardEnergyCost(cardData.cardEnergyCost, player, cardData.cardType);

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

    private bool TryPlayAttack(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy, int cardEnergyCost)
    {
        var attackCard = runtimeCard.cardData as Attack;
        if (attackCard == null) return false;

        var scaledDamage = CalculateScaledAttackDamage(player, attackCard);

        var finalAttackDamage = player.GetModifiedAttackDamage(scaledDamage);

        BeginCardPlay(player, attackCard, cardEnergyCost);
        
        player.ClearNextAttackEnergyReduction();

        Debug.Log(
            $"Played attack card: {attackCard.cardName}," +
            $" Base Damage: {attackCard.cardDamage}," +
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

                    for (var i = 0; i < attackCard.hitCount; i++)
                    {
                        enemy.TakeDamage(finalAttackDamage, true, player);
                    }
                }

                break;
            }

            case Card.TargetType.RandomEnemy:
            {
                for (var i = 0; i < attackCard.hitCount; i++)
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
                for (var i = 0; i < attackCard.hitCount; i++)
                {
                    targetEnemy.TakeDamage(finalAttackDamage, true, player);
                }

                break;
            }
        }

        player.ProcessCardTypeTriggeredEffects(attackCard.cardType);
        ApplyCardStatus(player, attackCard, targetEnemy);
        CompleteCardPlay(runtimeCard, cardObject, player);

        return true;
    }

    private bool TryPlayDefense(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy, int cardEnergyCost)
    {
        var defenseCard = runtimeCard.cardData as Defense;
        if (defenseCard == null) return false;

        BeginCardPlay(player, defenseCard, cardEnergyCost);
        ApplyCardStatus(player, defenseCard, targetEnemy);

        var finalBlockToGain = CalculateFinalBlock(defenseCard);

        player.GainBlock(finalBlockToGain);
        
        CompleteCardPlay(runtimeCard, cardObject, player);
        
        return true;
    }

    private bool TryPlayUtility(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy, int cardEnergyCost)
    {
        var utilityCard = runtimeCard.cardData as UtilityCard;
        if (utilityCard == null) return false;

        BeginCardPlay(player, utilityCard, cardEnergyCost);
        ApplyCardStatus(player, utilityCard, targetEnemy);
        ProcessNextCardEnergyReduction(utilityCard, player);

        if (utilityCard.cardEnergyGain > 0)
        {
            player.GainEnergy(utilityCard.cardEnergyGain);
        }

        if (utilityCard.cardHealthGain > 0)
        {
            player.Heal(utilityCard.cardHealthGain);
        }

        CompleteCardPlay(runtimeCard, cardObject, player);

        return true;
    }

    private bool TryPlayPower(Player player, RuntimeCard runtimeCard, GameObject cardObject, int cardEnergyCost)
    {
        var powerCard = runtimeCard.cardData as Power;
        if (powerCard == null) return false;

        BeginCardPlay(player, powerCard, cardEnergyCost);
        ApplyCardStatus(player, powerCard, null);
        CompleteCardPlay(runtimeCard, cardObject, player);

        return true;
    }

    private void CompleteCardPlay(RuntimeCard runtimeCard, GameObject cardObject, Player player)
    {
        var cardData = runtimeCard.cardData;

        ApplyCardHealthLoss(player, cardData);
        DrawCardsFromCard(cardData);
        ApplyRandomCardDiscard(cardData);
        DrawRandomCardFromDiscard(cardData);
        ApplyCardBonusEnergy(player, cardData);

        player.ProcessOnActionStatuses();

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
                ExhaustCard(runtimeCard, cardObject);
                break;
        }
        
        ResolveCardCreation(cardData);
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

    private void ExhaustCard(RuntimeCard runtimeCard, GameObject cardObject)
    {
        _handManager.RemoveCardFromHand(cardObject);
        _exhaustManager.AddToExhaustPile(runtimeCard);
        
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

    private void BeginCardPlay(Player player, Card cardData, int cardEnergyCost)
    {
        ApplyCardCorruption(player, cardData);
        SpendCardEnergy(player, cardEnergyCost);
    }

    private void SpendCardEnergy(Player player, int cardEnergyCost)
    {
        if (cardEnergyCost > 0)
        {
            player.SpendEnergy(cardEnergyCost);
        }
    }

    private void ApplyCardCorruption(Player player, Card cardData)
    {
        if (cardData.cardCorruptionGain > 0)
        {
            player.GainCorruption(cardData.cardCorruptionGain);
        }
    }

    private int CalculateScaledAttackDamage(Player player, Attack cardData)
    {
        var baseDamage = cardData.cardDamage;
        var scaledDamage = baseDamage;
        
        if (cardData.scalesWithCorruption && cardData.corruptionDamagePerPoint > 0)
        {
            var corruptionBonus = player.playerCorruption * cardData.corruptionDamagePerPoint;
            scaledDamage += corruptionBonus;
        }

        return scaledDamage;
    }
    
    private int CalculateFinalBlock(Defense cardData)
    {
        var baseBlock   = cardData.cardBlock;
        var scaledBlock = baseBlock;
        
        // Blood Guard
        if (_enemyManager.DoesAnyEnemyHaveStatus(StatusEffect.StatusType.Bleed) && cardData.bonusBlockIfEnemyHasBleed > 0)
        {
            scaledBlock += cardData.bonusBlockIfEnemyHasBleed;
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

    private void ApplyCardHealthLoss(Player player, Card cardData)
    {
        if (cardData.cardHealthLoss > 0)
        {
            player.LoseHealth(cardData.cardHealthLoss);
        }
    }

    private void DrawCardsFromCard(Card cardData)
    {
        if (cardData.cardsToDraw > 0)
        {
            _handManager.DrawCards(cardData.cardsToDraw);
        }
    }

    private void ApplyRandomCardDiscard(Card cardData)
    {
        if (cardData.cardsToDiscardRandomly > 0)
        {
            _handManager.DiscardRandomCards(
                cardData.cardsToDiscardRandomly
            );
        }
    }

    private void DrawRandomCardFromDiscard(Card cardData)
    {
        if (cardData.cardsToDrawFromDiscard <= 0) return;

        for (var i = 0; i < cardData.cardsToDrawFromDiscard; i++)
        {
            if (_handManager.IsHandFull()) break;

            var runtimeCard =
                _discardManager.PullRandomCardFromDiscard();

            if (runtimeCard == null) break;

            _handManager.AddCardToHand(runtimeCard);
        }
    }

    private void ApplyCardStatus(Player player, Card cardData, Enemy targetEnemy)
    {
        if (!cardData.appliesStatus) return;
        
        var statusEffect = new StatusEffect
        {
            statusType = cardData.statusType,
            amount = cardData.statusAmount,
            duration = cardData.statusDuration
        };
        
        if (cardData is Power powerCard && powerCard.statusToCreate != null)
        {
            statusEffect.statusToCreate = new StatusEffect
            {
                statusType = powerCard.statusToCreate.statusType,
                amount     = powerCard.statusToCreate.amount,
                duration   = powerCard.statusToCreate.duration
            };
        }

        switch (cardData.targetType)
        {
            case Card.TargetType.Self:
                player.ApplyStatus(statusEffect);
                break;

            case Card.TargetType.SingleEnemy:
            {
                if (targetEnemy == null) break;

                targetEnemy.ApplyStatus(statusEffect);

                if (statusEffect.statusType == StatusEffect.StatusType.Bleed)
                {
                    player.ProcessBleedAppliedTriggerEffects(targetEnemy);
                }

                break;
            }
        }
    }

    private void ProcessNextCardEnergyReduction(Card cardData, Player player)
    {
        if (!cardData.reducesNextAttackEnergy) return;

        var nextEnergyReduction = cardData.energyToReduce;

        player.AddNextAttackEnergyReduction(nextEnergyReduction);
    }

    private void ResolveCardCreation(Card cardData)
    {
        if (cardData == null) return;
        if (!cardData.createsCards) return;

        for (var i = 0; i < cardData.cardsToCreate; i++)
        {
            _deckManager.CreateCardDuringCombat(cardData.cardToCreate, cardData.createdCardDestination);
        }
    }

    private void ApplyCardBonusEnergy(Player player, Card cardData)
    {
        if (!player.HasStatus(StatusEffect.StatusType.EndlessAssault)) return;
        if (player.endlessAssaultTriggeredThisTurn)
        {
            Debug.Log("Already played a multihit attack, cannot gain more energy this turn!");
            return;
        }

        if (cardData is Attack attackCard && attackCard.hitCount > 1)
        {
            var energyToGain = player.GetStatusAmount(StatusEffect.StatusType.EndlessAssault);
            player.GainEnergy(energyToGain);
            player.TriggerEndlessAssault();
        }
    }
}