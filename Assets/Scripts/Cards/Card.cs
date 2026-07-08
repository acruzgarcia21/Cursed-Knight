using UnityEngine;

namespace CursedKnight
{
    public class Card : ScriptableObject
    {
        [Header("General")] 
        public string cardName;
        public string cardDescription;
        
        [Space(10)] [Header("Card Info")]
        public CardType cardType;
        public TargetType targetType;
        public Sprite cardSprite;
        
        [Space(10)] [Header("Card Effects")]
        public int cardEnergyCost;
        public int cardCorruptionGain;
        public int cardsToDraw;
        public int cardsToDiscardRandomly;
        public int cardsToDrawFromDiscard;
        
        [Space(10)] [Header("Status Effects")]
        public bool appliesStatus;
        public StatusEffect.StatusType statusType;
        public int statusAmount;
        public int statusDuration;



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
