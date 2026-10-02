using UnityEngine;

[System.Serializable]
public class CardSaveData
{
    [SerializeField] private string cardID;

    [SerializeField] private bool isUpgraded;

    public CardSaveData(string cardID, bool isUpgraded)
    {
        this.cardID     = cardID;
        this.isUpgraded = isUpgraded;
    }
    
    public string GetCardID()
    {
        return cardID;
    }

    public bool GetIsUpgraded()
    {
        return isUpgraded;
    }
}
