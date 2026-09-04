using CursedKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public RuntimeCard runtimeCard;

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

        cardName.text = cardData.cardName;
        cardEnergyCost.text = cardData.cardEnergyCost.ToString();
        cardDescription.text = cardData.cardDescription;
        cardCorruptionGain.text = cardData.cardCorruptionGain.ToString();
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
}