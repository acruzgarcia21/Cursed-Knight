using CursedKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [System.NonSerialized] public RuntimeCard runtimeCard;
    
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardEnergyCost;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private TMP_Text cardCorruptionGain;
    [SerializeField] private TMP_Text cardType;
    
    [SerializeField] private Image cardFrame;

    [SerializeField] private Sprite attackCardFrame;
    [SerializeField] private Sprite defenseCardFrame;
    [SerializeField] private Sprite utilityCardFrame;
    [SerializeField] private Sprite powerCardFrame;

    private void Start()
    {
        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {
        if (runtimeCard == null || runtimeCard.cardData == null)
        {
            Debug.LogWarning("CardDisplay has no RuntimeCard data.");
            return;
        }

        var cardData = runtimeCard.cardData;
        var isUpgraded = runtimeCard.isUpgraded;

        cardName.text = runtimeCard.isUpgraded ? $"{cardData.cardName}+" : cardData.cardName;
        cardEnergyCost.text = cardData.GetCardEnergyCost(isUpgraded).ToString();
        cardDescription.text = GetCardDescription(cardData, isUpgraded);
        cardCorruptionGain.text = cardData.GetCardCorruptionGain(isUpgraded).ToString();
        cardType.text = cardData.cardType.ToString();

        if (cardFrame != null)
        {
            cardFrame.sprite = cardData.cardType switch
            {
                Card.CardType.Attack => attackCardFrame,
                Card.CardType.Defense => defenseCardFrame,
                Card.CardType.Utility => utilityCardFrame,
                Card.CardType.Power => powerCardFrame,
                _ => cardFrame.sprite
            };
        }
    }

    private string GetCardDescription(Card cardData, bool isUpgraded)
    {
        var description = cardData.cardDescription;

        description = description
            .Replace("{corruption}", cardData.GetCardCorruptionGain(isUpgraded).ToString())
            .Replace("{draw}", cardData.GetCardsToDraw(isUpgraded).ToString())
            .Replace("{discard}", cardData.GetCardsToDiscardRandomly(isUpgraded).ToString())
            .Replace("{drawFromDiscard}", cardData.GetCardsToDrawFromDiscard(isUpgraded).ToString())
            .Replace("{healthGain}", cardData.GetCardHealthGain(isUpgraded).ToString())
            .Replace("{healthLoss}", cardData.GetCardHealthLoss(isUpgraded).ToString())
            .Replace("{energyGain}", cardData.GetCardEnergyGain(isUpgraded).ToString())
            .Replace("{statusAmount}", cardData.GetStatusAmount(isUpgraded).ToString())
            .Replace("{statusDuration}", cardData.GetStatusDuration(isUpgraded).ToString())
            .Replace("{cardsCreated}", cardData.GetCardsToCreate(isUpgraded).ToString())
            .Replace("{energyReduction}", cardData.GetEnergyToReduce(isUpgraded).ToString());

        if (cardData is Attack attack)
        {
            description = description
                .Replace("{damage}", attack.GetAttackDamage(isUpgraded).ToString())
                .Replace("{hitCount}", attack.GetHitCount(isUpgraded).ToString())
                .Replace("{corruptionDamage}", attack.GetCorruptionDamagePerPoint(isUpgraded).ToString());
        }

        if (cardData is Defense defense)
        {
            description = description
                .Replace("{block}", defense.GetCardBlock(isUpgraded).ToString())
                .Replace("{bonusBlock}", defense.GetBonusBlockIfEnemyHasBleed(isUpgraded).ToString());
        }

        if (cardData is Power power && power.statusToCreate != null)
        {
            description = description
                .Replace("{powerStatusAmount}", power.statusToCreate.GetStatusAmount(isUpgraded).ToString())
                .Replace("{powerStatusDuration}", power.statusToCreate.GetStatusDuration(isUpgraded).ToString());
        }

        return description;
    }
}