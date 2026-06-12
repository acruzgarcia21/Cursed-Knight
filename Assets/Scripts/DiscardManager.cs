using System.Collections.Generic;
using TMPro;
using CursedKnight;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    [SerializeField] public List<Card> discardPile = new List<Card>();
    public TextMeshProUGUI discardCount;
    public int discardCardsCount;

    private void Awake()
    {
        UpdateDiscardCount();
    }

    private void UpdateDiscardCount()
    {
        discardCount.text = discardPile.Count.ToString();
        discardCardsCount = discardPile.Count;
    }

    public void AddToDiscardPile(Card card)
    {
        if (card == null) return;
        
        discardPile.Add(card);
        UpdateDiscardCount();
    }

    public Card PullFromDiscardPile()
    {
        if (!IsDiscardPileEmpty())
        {
            var cardToReturn = discardPile[discardPile.Count - 1];
            discardPile.RemoveAt(discardPile.Count - 1);
            UpdateDiscardCount();
            return cardToReturn;
        }
        else
        {
            return null;
        }
    }

    public bool SelectCardFromDiscardPile(Card card)
    {
        if (!IsDiscardPileEmpty() && discardPile.Contains(card))
        {
            discardPile.Remove(card);
            UpdateDiscardCount();
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Card> PullAllFromDiscardPile()
    {
        if (!IsDiscardPileEmpty())
        {
            var cardsToReturn = new List<Card>(discardPile);
            discardPile.Clear();
            UpdateDiscardCount();
            return cardsToReturn;
        }
        else
        {
            return new List<Card>();
        }
    }

    private bool IsDiscardPileEmpty()
    {
        return discardPile.Count == 0;
    }
    
}
