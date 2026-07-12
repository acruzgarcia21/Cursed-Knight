using System.Collections.Generic;
using UnityEngine;
using CursedKnight;

[CreateAssetMenu(fileName = "New Deck", menuName = "Deck")]
public class DeckData : ScriptableObject
{
    [SerializeField] private List<Card> deckCards = new();

    public IReadOnlyList<Card> GetPlayerDeck()
    {
        return deckCards;
    }
}
