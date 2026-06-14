using UnityEngine;
using CursedKnight;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    
    // All Card Elements
    public Card cardData;
    
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
    // Updates all card data populated by each card in player's hand/deck
    private void UpdateCardDisplay()
    {
        cardName.text = cardData.name;
        cardEnergyCost.text = cardData.cardEnergyCost.ToString();
        cardDescription.text = cardData.cardDescription;
        cardCorruptionGain.text = cardData.cardCorruptionGain.ToString();
        cardType.text = cardData.cardType.ToString();

        // Future use
        // cardSprite = cardData.cardSprite;
    }
}
