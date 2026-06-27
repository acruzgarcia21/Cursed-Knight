using System;
using CursedKnight;
using UnityEngine;

public class CardPlayManager : MonoBehaviour
{
    private HandManager _handManager;
    private DiscardManager _discardManager;

    private void Awake()
    {
        _handManager    = FindFirstObjectByType<HandManager>();
        _discardManager = FindFirstObjectByType<DiscardManager>();
    }

    public bool TryPlayCard(Player player, Card cardData, GameObject cardObject)
    {
        if (cardData == null) return false;
        
        if (player.playerEnergy < cardData.cardEnergyCost)
        {
            Debug.Log("Not enough energy!");
            return false;
        }

        return cardData.cardType switch
        {
            Card.CardType.Attack => TryPlayAttack(player, cardData, cardObject),
            Card.CardType.Defense => TryPlayDefense(player, cardData, cardObject),
            Card.CardType.Utility => TryPlayUtility(player, cardData, cardObject),
            _ => false
        };
    }

    private bool TryPlayAttack(Player player, Card cardData, GameObject cardObject)
    {
        var attackCard = cardData as Attack;
        if (attackCard == null) return false;

        var enemy = BattleManager.Instance.EnemyManager.GetFirstLivingEnemy();
        if (enemy == null) return false;
        
        if (attackCard.cardCorruptionGain > 0)
        {
            player.GainCorruption(attackCard.cardCorruptionGain);
        }

        if (attackCard.cardEnergyCost > 0)
        {
            player.SpendEnergy(attackCard.cardEnergyCost);
        }
        
        Debug.Log($"Played attack card: {attackCard.cardName}, Damage: {attackCard.cardDamage}");
        
        enemy.TakeDamage(attackCard.cardDamage);
        
        SendCardToDiscard(cardData, cardObject);
        return true;
    }

    private bool TryPlayDefense(Player player, Card cardData, GameObject cardObject)
    {
        var defenseCard = cardData as Defense;
        if (defenseCard == null) return false;

        player.GainBlock(defenseCard.cardBlock);
        
        if (defenseCard.cardCorruptionGain > 0)
        {
            player.GainCorruption(defenseCard.cardCorruptionGain);
        }
        
        if (defenseCard.cardEnergyCost > 0)
        {
            player.SpendEnergy(defenseCard.cardEnergyCost);
        }

        Debug.Log($"Played defense card: {defenseCard.cardName}, Block: {defenseCard.cardBlock}");
        SendCardToDiscard(cardData, cardObject);
        return true;
    }

    private bool TryPlayUtility(Player player, Card cardData, GameObject cardObject)
    {
        var utilityCard = cardData as UtilityCard;
        if (utilityCard == null) return false;

        if (utilityCard.cardEnergyGain > 0)
        {
            player.GainEnergy(utilityCard.cardEnergyGain);
        }
        
        if (utilityCard.cardHealthGain > 0)
        {
            player.Heal(utilityCard.cardHealthGain);
        }

        if (utilityCard.cardCorruptionGain > 0)
        {
            player.GainCorruption(utilityCard.cardCorruptionGain);
        }
        
        if (utilityCard.cardEnergyCost > 0)
        {
            player.SpendEnergy(utilityCard.cardEnergyCost);
            
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
}
