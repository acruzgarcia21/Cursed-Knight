using UnityEngine;

namespace CursedKnight
{
    public class Card : ScriptableObject
    {
        [Header("General")] 
        public string cardName;
        public string cardDescription;
        [SerializeField] private string cardID;
        
        [Space(10)] 
        [Header("Card Info")]
        public CardType cardType;
        public TargetType targetType;
        public Sprite cardSprite;
        
        [Space(10)] 
        [Header("Card Effects")]
        public int cardEnergyCost;
        public int cardCorruptionGain;
        public int cardsToDraw;
        public int cardsToDiscardRandomly;
        public int cardsToDrawFromDiscard;
        public int cardHealthGain;
        public int cardHealthLoss;
        public int cardEnergyGain;

        [Space(10)] 
        [Header("Status Effects")] 
        public bool appliesStatus;
        public StatusTargetType statusTargetType;
        public StatusEffect.StatusType statusType;
        public int statusAmount;
        public int statusDuration;
        
        [Space(10)] 
        [Header("Runtime Types")]
        public bool retain;
        public bool exhaust;
        public bool spectral;

        [Space(10)] 
        [Header("Created During Combat")]
        public bool createsCards;
        public Card cardToCreate;
        public int cardsToCreate;
        public CreatedCardDestination createdCardDestination;

        [Space(10)] 
        [Header("Energy Reduction")]
        public bool reducesNextAttackEnergy;

        public int energyToReduce;
        public enum CreatedCardDestination
        {
            Hand,
            DrawPile,
            DiscardPile
        }
        

        public enum CardType
        {
            Attack,
            Defense,
            Utility,
            Power
        }

        public enum TargetType
        {
            SingleEnemy,
            AllEnemies,
            RandomEnemy,
            Self,
            None
        }

        public enum StatusTargetType
        {
            SingleEnemy,
            AllEnemies,
            RandomEnemy,
            Self
        }

        public string GetCardID()
        {
            return cardID;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(cardID)) return;

            cardID = GenerateId(name);
        }

        private string GenerateId(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName)) return string.Empty;

            var id = assetName.Trim().ToLowerInvariant();

            id = id.Replace("'", "");
            id = id.Replace("’", "");
            id = id.Replace("-", "_");
            id = id.Replace(" ", "_");

            while (id.Contains("__"))
            {
                id = id.Replace("__", "_");
            }

            return id;
        }
        #endif

        [Space(10)]
        [Header("UPGRADE ATTRIBUTES")]

        [Header("Upgrade - General")]
        [SerializeField] private int upgradedEnergyCost;
        [SerializeField] private int upgradedCorruptionGain;

        [Header("Upgrade - Card Effects")]
        [SerializeField] private int upgradedCardsToDraw;
        [SerializeField] private int upgradedCardsToDiscardRandomly;
        [SerializeField] private int upgradedCardsToDrawFromDiscard;
        [SerializeField] private int upgradedCardHealthGain;
        [SerializeField] private int upgradedCardHealthLoss;
        [SerializeField] private int upgradedCardEnergyGain;

        [Header("Upgrade - Status Effects")]
        [SerializeField] private int upgradedStatusAmount;
        [SerializeField] private int upgradedStatusDuration;

        [Header("Upgrade - Created Cards")]
        [SerializeField] private int upgradedCardsToCreate;

        [Header("Upgrade - Energy Reduction")]
        [SerializeField] private int upgradedEnergyToReduce;
        
        public int GetCardEnergyCost(bool isUpgraded)
        {
            return isUpgraded ? upgradedEnergyCost : cardEnergyCost;
        }

        public int GetCardCorruptionGain(bool isUpgraded)
        {
            return isUpgraded ? upgradedCorruptionGain : cardCorruptionGain;
        }

        public int GetCardsToDraw(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardsToDraw : cardsToDraw;
        }

        public int GetCardsToDiscardRandomly(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardsToDiscardRandomly : cardsToDiscardRandomly;
        }

        public int GetCardsToDrawFromDiscard(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardsToDrawFromDiscard : cardsToDrawFromDiscard;
        }

        public int GetCardHealthGain(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardHealthGain : cardHealthGain;
        }

        public int GetCardHealthLoss(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardHealthLoss : cardHealthLoss;
        }

        public int GetCardEnergyGain(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardEnergyGain : cardEnergyGain;
        }

        public int GetStatusAmount(bool isUpgraded)
        {
            return isUpgraded ? upgradedStatusAmount : statusAmount;
        }

        public int GetStatusDuration(bool isUpgraded)
        {
            return isUpgraded ? upgradedStatusDuration : statusDuration;
        }

        public int GetCardsToCreate(bool isUpgraded)
        {
            return isUpgraded ? upgradedCardsToCreate : cardsToCreate;
        }

        public int GetEnergyToReduce(bool isUpgraded)
        {
            return isUpgraded ? upgradedEnergyToReduce : energyToReduce;
        }
        
        
    }
    
    
}


