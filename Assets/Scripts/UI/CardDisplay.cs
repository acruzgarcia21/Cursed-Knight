using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public RuntimeCard runtimeCard;

    public Image cardSprite;

    public TMP_Text cardName;
    public TMP_Text cardEnergyCost;
    public TMP_Text cardDescription;
    public TMP_Text cardCorruptionGain;
    public TMP_Text cardType;

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

        if (cardSprite != null)
        {
            cardSprite.sprite = cardData.cardSprite;
        }
    }
}