using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    public Enemy enemy;

    public TMP_Text enemyName;
    public TMP_Text enemyHealth;
    public TMP_Text enemyBlock;

    public Image enemySprite;

    [SerializeField] private GameObject blockDisplay;
    
    [SerializeField] private Sprite attackIcon;
    [SerializeField] private Sprite blockIcon;
    [SerializeField] private Sprite healIcon;
    [SerializeField] private Sprite buffIcon;
    [SerializeField] private Sprite debuffIcon;
    [SerializeField] private Sprite bonusDamageIcon;
    [SerializeField] private Sprite hideIntentIcon;
    [SerializeField] private Sprite summonEnemyIcon;

    [SerializeField] private IntentEntryDisplay[] intentEntries;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemySprite = transform.Find("EnemyCanvas/EnemyImage").GetComponent<Image>();
    }

    public void UpdateEnemyDisplay()
    {
        if (enemy == null || enemy.enemyData == null)
        {
            return;
        }

        enemyName.text     = enemy.enemyData.enemyName;
        enemyHealth.text   = enemy.currentEnemyHealth + " / " + enemy.enemyData.enemyMaxHealth;
        enemySprite.sprite = enemy.enemyData.enemySprite;

        UpdateBlockDisplay();
        UpdateIntentDisplay();
    }

    private void UpdateBlockDisplay()
    {
        if (blockDisplay == null || enemyBlock == null) return;

        var hasBlock = enemy.currentEnemyBlock > 0;
        
        blockDisplay.SetActive(hasBlock);

        if (hasBlock)
        {
            enemyBlock.text = enemy.currentEnemyBlock.ToString();
        }
    }

    private void UpdateIntentDisplay()
    {
        foreach (var intentEntryDisplay in intentEntries)
        {
            intentEntryDisplay.Clear();
        }

        if (enemy.CurrentAction == null) return;

        var entryIndex = 0;

        if (enemy.CurrentAction.damage > 0)
        {
            var intentDamage = enemy.GetCurrentIntentDamage();
            var hitCount = Mathf.Max(1, enemy.CurrentAction.hitCount);

            TooltipData tooltipData;
            
            if (hitCount == 1)
            {
                tooltipData = new TooltipData(
                    "Attack", 
                    "This enemy intends to deal " + intentDamage + " damage."
                );
            }
            else
            {
                tooltipData = new TooltipData(
                    "Attack", 
                    "This enemy intends to deal " + intentDamage + " damage " + hitCount + " times."
                );
            }

            var formattedDamage = hitCount > 1
                ? $"{intentDamage} x {hitCount}"
                : intentDamage.ToString();

            AddIntentEntry(attackIcon, formattedDamage, tooltipData, ref entryIndex);
        }

        if (enemy.CurrentAction.blockAmount > 0)
        {
            var tooltipData = new TooltipData(
                "Block", 
                "This enemy intends to gain " + enemy.CurrentAction.blockAmount + " Block."
            );
            
            AddIntentEntry(blockIcon, enemy.CurrentAction.blockAmount.ToString(), tooltipData, ref entryIndex);
        }
        
        if (enemy.CurrentAction.healingAmount > 0)
        {
            var tooltipData = new TooltipData(
                "Heal", 
                "This enemy intends to heal for " + enemy.CurrentAction.healingAmount + " HP."
            );
            
            AddIntentEntry(healIcon, enemy.CurrentAction.healingAmount.ToString(), tooltipData, ref entryIndex);
        }

        if (enemy.CurrentAction.nextAttackBonusDamage > 0)
        {
            var tooltipData = new TooltipData(
                "Bonus Damage", 
                "This enemy intends to increase the damage of its next attack by " 
                + enemy.CurrentAction.nextAttackBonusDamage + "."
            );
            
            AddIntentEntry(bonusDamageIcon, "", tooltipData, ref entryIndex);
        }
        
        if (enemy.CurrentAction.hidesEnemy)
        {
            var tooltipData = new TooltipData(
                "Hide", 
                "This enemy intends to hide."
            );
            
            AddIntentEntry(hideIntentIcon, "", tooltipData, ref entryIndex);
        }
        
        if (enemy.CurrentAction.enemyToSummon != null && enemy.CurrentAction.enemiesToSummon > 0)
        {
            var tooltipData = new TooltipData(
                "Summon", 
                "This enemy intends to summon " + enemy.CurrentAction.enemiesToSummon + " allies."
            );
            
            AddIntentEntry(
                summonEnemyIcon, 
                enemy.CurrentAction.enemiesToSummon.ToString(), 
                tooltipData,
                ref entryIndex
                );
        }

        if (enemy.CurrentAction.appliesStatus && enemy.CurrentAction.statusAmount > 0)
        {
            switch (enemy.CurrentAction.statusTarget)
            {
                case EnemyActionData.StatusTargetType.Self:
                case EnemyActionData.StatusTargetType.RandomAlly:
                case EnemyActionData.StatusTargetType.AllOtherAllies:
                    var tooltipBuffData = new TooltipData(
                        "Buff", 
                        "This enemy intends to buff."
                    );
                    
                    AddIntentEntry(
                        buffIcon, 
                        enemy.CurrentAction.statusDuration.ToString(), 
                        tooltipBuffData, 
                        ref entryIndex
                        );
                    
                    break;
                case EnemyActionData.StatusTargetType.Player:
                    var tooltipDebuffData = new TooltipData(
                        "Debuff", 
                        "This enemy intends to debuff."
                    );
                    
                    AddIntentEntry(
                        debuffIcon, 
                        enemy.CurrentAction.statusAmount.ToString(), 
                        tooltipDebuffData, 
                        ref entryIndex
                        );
                    
                    break;
            }
        }
    }
    
    private void AddIntentEntry(Sprite icon, string text, TooltipData tooltipData, ref int entryIndex)
    {
        if (icon == null || intentEntries == null || entryIndex >= intentEntries.Length)
        {
            return;
        }

        intentEntries[entryIndex].DisplayIntent(icon, text, tooltipData);
        entryIndex++;
    }
    
}