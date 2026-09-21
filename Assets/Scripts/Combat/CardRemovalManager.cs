using CursedKnight;
using UnityEngine;

public class CardRemovalManager : MonoBehaviour
{
    private Card _selectedCard;
    
    public void SelectCard(Card cardToRemove)
    {
        _selectedCard = cardToRemove;
        Debug.Log("Selected Card: " + _selectedCard + ". Card To Remove: " + cardToRemove + ".");
    }
}
