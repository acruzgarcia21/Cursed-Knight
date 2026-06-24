using UnityEngine;

namespace CursedKnight
{
    public class Card : ScriptableObject
    {
        public string cardName;
        public string cardDescription;
        
        public CardType cardType;
        
        public Sprite cardSprite;
        
        public int cardEnergyCost;
        public int cardCorruptionGain;

        public enum CardType
        {
            Attack,
            Defense,
            Utility
        }
    }
}
