using UnityEngine;

namespace CursedKnight
{
    public class Card : ScriptableObject
    {
        public string cardName;
        public string cardDescription;
        
        public CardType cardType;

        public TargetType targetType;
        
        public Sprite cardSprite;
        
        public int cardEnergyCost;
        public int cardCorruptionGain;
        public int cardsToDraw;
        public int cardsToDiscardRandomly;

        public enum CardType
        {
            Attack,
            Defense,
            Utility
        }

        public enum TargetType
        {
            SingleEnemy,
            AllEnemies,
            RandomEnemy,
            Self,
            None
        }
    }
}
