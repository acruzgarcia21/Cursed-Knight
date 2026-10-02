using System.Collections.Generic;
using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Database", menuName = "CardDatabase")]

public class CardDatabase : ScriptableObject
{
    [SerializeField] private List<Card> persistentCardDefinitions;
        
    private readonly Dictionary<string, Card> _cardDictionary = new();

    private void OnEnable()
    {
        _cardDictionary.Clear();
        
        foreach (var card in persistentCardDefinitions)
        {
            if (card == null || string.IsNullOrEmpty(card.GetCardID()))
            {
                Debug.LogError("Card is not Valid!");
                continue;
            }

            if (_cardDictionary.ContainsKey(card.GetCardID()))
            {
                Debug.LogError("Dictionary already contains card!");
                continue;
            }
            
            _cardDictionary.Add(card.GetCardID(), card);
        }
    }

    public Card GetCardByID(string cardID)
    {
        if (_cardDictionary.TryGetValue(cardID, out var card))
        {
            return card;
        }

        Debug.LogError("Dictionary does not contain the card associated with the provided key!");
        return null;
    }
}
