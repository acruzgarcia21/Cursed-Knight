using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

public class CardPlayManager : MonoBehaviour
{
    private HandManager _handManager;
    private DiscardManager _discardManager;
    private EnemyManager _enemyManager;

    private void Awake()
    {
        _handManager    = FindFirstObjectByType<HandManager>();
        _discardManager = FindFirstObjectByType<DiscardManager>();
        _enemyManager   = FindFirstObjectByType<EnemyManager>();
    }

    public bool TryPlayCard(Player player, Card cardData, GameObject cardObject, Enemy targetEnemy)
    {
        if (cardData == null) return false;
        
        if (player.playerEnergy < cardData.cardEnergyCost)
        {
            Debug.Log("Not enough energy!");
            return false;
        }

        if (!IsTargetValid(player, cardData, targetEnemy))
        {
            Debug.Log("Invalid Target");
            return false;
        }

        return cardData.cardType switch
        {
            Card.CardType.Attack => TryPlayAttack(player, cardData, cardObject, targetEnemy),
            Card.CardType.Defense => TryPlayDefense(player, cardData, cardObject),
            Card.CardType.Utility => TryPlayUtility(player, cardData, cardObject),
            _ => false
        };
    }

    private bool TryPlayAttack(Player player, Card cardData, GameObject cardObject, Enemy targetEnemy)
    {
        var attackCard = cardData as Attack;
        if (attackCard == null) return false;
        
        ApplyCardCorruption(player, attackCard);
        SpendCardEnergy(player, attackCard);
        
        Debug.Log($"Played attack card: {attackCard.cardName}, Damage: {attackCard.cardDamage}");
        
        switch (cardData.targetType)
        {
            case Card.TargetType.AllEnemies:
            {
                var allLivingEnemies = _enemyManager.GetLivingEnemies();
                foreach (var enemy in allLivingEnemies)
                {
                    enemy.TakeDamage(attackCard.cardDamage);
                }

                break;
            }
            case Card.TargetType.RandomEnemy:
            {
                var allLivingEnemies = _enemyManager.GetLivingEnemies();
                var randomEnemyIndex = Random.Range(0, allLivingEnemies.Count);
                allLivingEnemies[randomEnemyIndex].TakeDamage(attackCard.cardDamage);
                break;
            }
            case Card.TargetType.SingleEnemy:
            default:
                targetEnemy.TakeDamage(attackCard.cardDamage);
                break;
        }
        
        SendCardToDiscard(cardData, cardObject);
        return true;
    }

    private bool TryPlayDefense(Player player, Card cardData, GameObject cardObject)
    {
        var defenseCard = cardData as Defense;
        if (defenseCard == null) return false;

        ApplyCardCorruption(player, defenseCard);
        SpendCardEnergy(player, defenseCard);
        
        player.GainBlock(defenseCard.cardBlock);

        Debug.Log($"Played defense card: {defenseCard.cardName}, Block: {defenseCard.cardBlock}, Energy Spent: {defenseCard.cardEnergyCost}");
        SendCardToDiscard(cardData, cardObject);
        return true;
    }

    private bool TryPlayUtility(Player player, Card cardData, GameObject cardObject)
    {
        var utilityCard = cardData as UtilityCard;
        if (utilityCard == null) return false;

        ApplyCardCorruption(player, utilityCard);
        SpendCardEnergy(player, utilityCard);
        
        if (utilityCard.cardEnergyGain > 0)
        {
            player.GainEnergy(utilityCard.cardEnergyGain);
        }
        
        if (utilityCard.cardHealthGain > 0)
        {
            player.Heal(utilityCard.cardHealthGain);
        }
        
        Debug.Log($"Played utility card: " +
                  $"{utilityCard.cardName}, " +
                  $"Health: {utilityCard.cardHealthGain}, " +
                  $"Energy: {utilityCard.cardEnergyGain}, " +
                  $"Draw Cards: {utilityCard.cardsToDraw}");
        
        SendCardToDiscard(cardData, cardObject);
        return true;
    }
    
    private void SendCardToDiscard(Card cardData, GameObject cardObject)
    {
        _handManager.RemoveCardFromHand(cardObject);
        _discardManager.AddToDiscardPile(cardData);
        Destroy(cardObject);
    }

    private bool IsTargetValid(Player player, Card cardData, Enemy targetEnemy)
    {
        if (cardData == null) return false;

        switch (cardData.targetType)
        {
            case Card.TargetType.SingleEnemy:
                return targetEnemy != null;
            case Card.TargetType.AllEnemies:
            case Card.TargetType.RandomEnemy:
                return !_enemyManager.AllEnemiesDead();
            case Card.TargetType.Self:
                return player != null;
            case Card.TargetType.None:
            default:
                return true;
        }
    }

    private void SpendCardEnergy(Player player, Card cardData)
    {
        if (cardData.cardEnergyCost > 0)
        {
            player.SpendEnergy(cardData.cardEnergyCost);
        }
    }

    private void ApplyCardCorruption(Player player, Card cardData)
    {
        if (cardData.cardCorruptionGain > 0)
        {
            player.GainCorruption(cardData.cardCorruptionGain);
        }
    }
}
