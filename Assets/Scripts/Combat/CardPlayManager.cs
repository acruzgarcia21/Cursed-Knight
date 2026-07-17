using CursedKnight;
using UnityEngine;

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
            Card.CardType.Attack  => TryPlayAttack(player, runtimeCard, cardObject, targetEnemy),
            Card.CardType.Defense => TryPlayDefense(player, runtimeCard, cardObject, targetEnemy),
            Card.CardType.Utility => TryPlayUtility(player, runtimeCard, cardObject, targetEnemy),
            Card.CardType.Power   => TryPlayPower(player, runtimeCard, cardObject),
            _ => false
        };
    }

    private bool TryPlayAttack(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy)
    {
        var attackCard = runtimeCard.cardData as Attack;
        if (attackCard == null) return false;

        var finalAttackDamage =
            player.GetModifiedAttackDamage(attackCard.cardDamage);

        BeginCardPlay(player, attackCard);

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

                    if (allLivingEnemies.Count == 0) break;

                    var randomEnemyIndex = Random.Range(0, allLivingEnemies.Count);

                    allLivingEnemies[randomEnemyIndex].TakeDamage(finalAttackDamage);
                }

                break;
            }

            case Card.TargetType.SingleEnemy:
            default:
            {
                for (var i = 0; i < attackCard.hitCount; i++)
                {
                    targetEnemy.TakeDamage(finalAttackDamage);
                }

                break;
            }
        }

        player.ProcessCardTypeTriggeredEffects(attackCard.cardType);
        ApplyCardStatus(player, attackCard, targetEnemy);
        CompleteCardPlay(runtimeCard, cardObject, player);

        return true;
    }

    private bool TryPlayDefense(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy)
    {
        var defenseCard = runtimeCard.cardData as Defense;
        if (defenseCard == null) return false;

        BeginCardPlay(player, defenseCard);
        ApplyCardStatus(player, defenseCard, targetEnemy);

        player.GainBlock(defenseCard.cardBlock);
        
        CompleteCardPlay(runtimeCard, cardObject, player);
        
        return true;
    }

    private bool TryPlayUtility(Player player, RuntimeCard runtimeCard, GameObject cardObject, Enemy targetEnemy)
    {
        var utilityCard = runtimeCard.cardData as UtilityCard;
        if (utilityCard == null) return false;

        BeginCardPlay(player, utilityCard);
        ApplyCardStatus(player, utilityCard, targetEnemy);

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

    private bool TryPlayPower(Player player, RuntimeCard runtimeCard, GameObject cardObject)
    {
        var powerCard = runtimeCard.cardData as Power;
        if (powerCard == null) return false;

        BeginCardPlay(player, powerCard);
        ApplyCardStatus(player, powerCard, null);
        
        CompleteCardPlay(runtimeCard, cardObject, player);

        return true;
    }

    private void CompleteCardPlay(RuntimeCard runtimeCard, GameObject cardObject, Player player)
    {
        var cardData = runtimeCard.cardData;

        DrawCardsFromCard(cardData);
        ApplyRandomCardDiscard(cardData);
        DrawRandomCardFromDiscard(cardData);

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

    private void BeginCardPlay(Player player, Card cardData)
    {
        ApplyCardCorruption(player, cardData);
        SpendCardEnergy(player, cardData);
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

        switch (cardData.targetType)
        {
            case Card.TargetType.Self:
                player.ApplyStatus(statusEffect);
                break;

            case Card.TargetType.SingleEnemy:
                if (targetEnemy != null)
                {
                    targetEnemy.ApplyStatus(statusEffect);
                }

                break;
        }
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
}