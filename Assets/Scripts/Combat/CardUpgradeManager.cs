using UnityEngine;

public class CardUpgradeManager : MonoBehaviour
{
    public event System.Action OnCardUpgraded;
    
    private RuntimeCard _selectedCard;

    public void SelectCard(RuntimeCard cardToUpgrade)
    {
        if (cardToUpgrade.isUpgraded) return;
        
        ClearSelectedCard();
        _selectedCard = cardToUpgrade;
    }

    public void ClearSelectedCard()
    {
        _selectedCard = null;
    }

    public void ResolveCardUpgradeConfirmation()
    {
        if (_selectedCard == null) return;

        _selectedCard.isUpgraded = true;
        
        ClearSelectedCard();
        
        OnCardUpgraded?.Invoke();
    }
}
