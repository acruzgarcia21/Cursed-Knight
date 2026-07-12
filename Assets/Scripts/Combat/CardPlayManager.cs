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
            Debug.Log("Invalid Target!");
            return false;
        }

        return cardData.cardType switch
        {
            Card.CardType.Attack  => TryPlayAttack(player, cardData, cardObject, targetEnemy),
            Card.CardType.Defense => TryPlayDefense(player, cardData, cardObject, targetEnemy),
            Card.CardType.Utility => TryPlayUtility(player, cardData, cardObject, targetEnemy),
            _ => false
        };
    }

    private bool TryPlayAttack(Player player, Card cardData, GameObject cardObject, Enemy targetEnemy)
    {
        var attackCard = cardData as Attack;
        if (attackCard == null) return false;

        var finalAttackDamage = player.GetModifiedAttackDamage(attackCard.cardDamage);
        
        BeginCardPlay(player, attackCard);
        
        Debug.Log($"Played attack card: {attackCard.cardName}," +
                  $" Base Damage: {attackCard.cardDamage}," +
                  $" Modified Damage: {finalAttackDamage}");
        
        switch (cardData.targetType)
        {
            case Card.TargetType.AllEnemies:
            {
                var allLivingEnemies = _enemyManager.GetLivingEnemies();
                foreach (var enemy in allLivingEnemies)
                {
                    for (var i = 0; i < attackCard.hitCount; i++)
                    {
                        enemy.TakeDamage(finalAttackDamage);
                    }
                }

                break;
            }
            case Card.TargetType.RandomEnemy:
            {
                for (var i = 0; i < attackCard.hitCount; i++)
                {
                    var allLivingEnemies = _enemyManager.GetLivingEnemies();
                    var randomEnemyIndex = Random.Range(0, allLivingEnemies.Count);
                    allLivingEnemies[randomEnemyIndex].TakeDamage(finalAttackDamage);
                }
                break;
            }
            case Card.TargetType.SingleEnemy:
            default:
                for (var i = 0; i < attackCard.hitCount; i++)
                {
                    targetEnemy.TakeDamage(finalAttackDamage);
                }
                break;
        }
        
        ApplyCardStatus(player, cardData, targetEnemy);
        CompleteCardPlay(attackCard, cardObject, player);
        
        return true;
    }

    private bool TryPlayDefense(Player player, Card cardData, GameObject cardObject, Enemy targetEnemy)
    {
        var defenseCard = cardData as Defense;
        if (defenseCard == null) return false;

        BeginCardPlay(player, defenseCard);
        ApplyCardStatus(player, cardData, targetEnemy); 
        
        player.GainBlock(defenseCard.cardBlock);

        CompleteCardPlay(defenseCard, cardObject, player);
        
        return true;
    }

    private bool TryPlayUtility(Player player, Card cardData, GameObject cardObject, Enemy targetEnemy)
    {
        var utilityCard = cardData as UtilityCard;
        if (utilityCard == null) return false;

        BeginCardPlay(player, utilityCard);
        ApplyCardStatus(player, cardData, targetEnemy);
        
        if (utilityCard.cardEnergyGain > 0)
        {
            player.GainEnergy(utilityCard.cardEnergyGain);
        }
        
        if (utilityCard.cardHealthGain > 0)
        {
            player.Heal(utilityCard.cardHealthGain);
        }
        
        CompleteCardPlay(utilityCard, cardObject, player);
        
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
            _handManager.DiscardRandomCards(cardData.cardsToDiscardRandomly);
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

        switch (cardData.targetType)
        {
            case Card.TargetType.Self:
                player.ApplyStatus(statusEffect);
                break;
            case Card.TargetType.SingleEnemy:
                targetEnemy.ApplyStatus(statusEffect);
                break;
        }
    }

    private void DrawRandomCardFromDiscard(Card cardData)
    {
        if (cardData.cardsToDrawFromDiscard <= 0) return;

        for (var i = 0; i < cardData.cardsToDrawFromDiscard; i++)
        {
            if (_handManager.IsHandFull()) break;

            var randomCardFromDiscard = _discardManager.PullRandomCardFromDiscard();

            if (randomCardFromDiscard == null) break;
            
            _handManager.AddCardToHand(randomCardFromDiscard);
        }
    }

    private void BeginCardPlay(Player player, Card cardData)
    {
        ApplyCardCorruption(player, cardData);
        SpendCardEnergy(player, cardData);
    }

    private void CompleteCardPlay(Card cardData, GameObject cardObject, Player player)
    {
        DrawCardsFromCard(cardData);
        ApplyRandomCardDiscard(cardData);
        DrawRandomCardFromDiscard(cardData);
        
        player.ProcessOnActionStatuses();
        
        SendCardToDiscard(cardData, cardObject);
    }
}
