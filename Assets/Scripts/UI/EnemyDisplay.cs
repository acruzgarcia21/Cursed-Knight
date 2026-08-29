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

    private Player _player;

    private void Awake()
    {
        enemy   = GetComponent<Enemy>();
        
        _player = FindFirstObjectByType<Player>();
        
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
        var isPlayerCorrupted = _player.HasStatus(StatusEffect.StatusType.Corruption);
        
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
                if (isPlayerCorrupted)
                {
                    tooltipData = new TooltipData(
                        "Attack", 
                        "This enemy intends to deal ??? damage."
                    );
                }
                else
                {
                    tooltipData = new TooltipData(
                        "Attack", 
                        "This enemy intends to deal " + intentDamage + " damage."
                    );
                }
            }
            else
            {
                if (isPlayerCorrupted)
                {
                    tooltipData = new TooltipData(
                        "Attack", 
                        "This enemy intends to deal ??? damage ??? times."
                    );
                }
                else
                {
                    tooltipData = new TooltipData(
                        "Attack", 
                        "This enemy intends to deal " + intentDamage + " damage " + hitCount + " times."
                    );
                }
            }

            var formattedDamage = hitCount > 1
                ? $"{intentDamage} x {hitCount}"
                : intentDamage.ToString();

            if (isPlayerCorrupted)
            {
                AddIntentEntry(attackIcon, "???", tooltipData, ref entryIndex);
            }
            else
            {
                AddIntentEntry(attackIcon, formattedDamage, tooltipData, ref entryIndex);
            }
        }

        if (enemy.CurrentAction.blockAmount > 0)
        {
            TooltipData tooltipData;
            if (isPlayerCorrupted)
            {
                tooltipData = new TooltipData(
                    "Block", 
                    "This enemy intends to gain ??? Block."
                );
            }
            else
            {
                tooltipData = new TooltipData(
                    "Block", 
                    "This enemy intends to gain " + enemy.CurrentAction.blockAmount + " Block."
                );   
            }
            
            if (isPlayerCorrupted)
            {
                AddIntentEntry(blockIcon, "???", tooltipData, ref entryIndex);
            }
            else
            {
                AddIntentEntry(blockIcon, enemy.CurrentAction.blockAmount.ToString(), tooltipData, ref entryIndex);
            }
        }
        
        if (enemy.CurrentAction.healingAmount > 0)
        {
            TooltipData tooltipData;

            if (isPlayerCorrupted)
            {
                tooltipData = new TooltipData(
                    "Heal", 
                    "This enemy intends to heal for ??? HP."
                );
            }
            else
            {
                tooltipData = new TooltipData(
                    "Heal", 
                    "This enemy intends to heal for " + enemy.CurrentAction.healingAmount + " HP."
                );
            }

            if (isPlayerCorrupted)
            {
                AddIntentEntry(healIcon, "???", tooltipData, ref entryIndex);
            }
            else
            {
                AddIntentEntry(healIcon, enemy.CurrentAction.healingAmount.ToString(), tooltipData, ref entryIndex);
            }
        }

        if (enemy.CurrentAction.nextAttackBonusDamage > 0)
        {
            TooltipData tooltipData;

            if (isPlayerCorrupted)
            {
                tooltipData = new TooltipData(
                    "Bonus Damage", 
                    "This enemy intends to increase the damage of its next attack by ???."
                );
            }
            else
            {
                tooltipData = new TooltipData(
                    "Bonus Damage", 
                    "This enemy intends to increase the damage of its next attack by " 
                    + enemy.CurrentAction.nextAttackBonusDamage + "."
                );
            }

            if (isPlayerCorrupted)
            {
                AddIntentEntry(bonusDamageIcon, "???", tooltipData, ref entryIndex);
            }
            else
            {
                AddIntentEntry(bonusDamageIcon, enemy.CurrentAction.nextAttackBonusDamage.ToString(), tooltipData, ref entryIndex);
            }
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
            TooltipData tooltipData;

            if (isPlayerCorrupted)
            {
                tooltipData = new TooltipData(
                    "Summon", 
                    "This enemy intends to summon ??? allies."
                );
            }
            else
            {
                tooltipData = new TooltipData(
                    "Summon", 
                    "This enemy intends to summon " + enemy.CurrentAction.enemiesToSummon + " allies."
                );
            }

            if (isPlayerCorrupted)
            {
                AddIntentEntry(
                    summonEnemyIcon, 
                    "???", 
                    tooltipData,
                    ref entryIndex
                );
            }
            else
            {
                AddIntentEntry(
                    summonEnemyIcon, 
                    enemy.CurrentAction.enemiesToSummon.ToString(), 
                    tooltipData,
                    ref entryIndex
                );
            }
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

                    if (isPlayerCorrupted)
                    {
                        AddIntentEntry(
                            buffIcon, 
                            "???", 
                            tooltipBuffData, 
                            ref entryIndex
                        );
                    }
                    else
                    {
                        AddIntentEntry(
                            buffIcon, 
                            enemy.CurrentAction.statusDuration.ToString(), 
                            tooltipBuffData, 
                            ref entryIndex
                        );   
                    }
                    
                    break;
                case EnemyActionData.StatusTargetType.Player:
                    var tooltipDebuffData = new TooltipData(
                        "Debuff", 
                        "This enemy intends to debuff."
                    );
                    
                    if (isPlayerCorrupted)
                    {
                        AddIntentEntry(
                            debuffIcon, 
                            "???", 
                            tooltipDebuffData, 
                            ref entryIndex
                        );
                    }
                    else
                    {
                        AddIntentEntry(
                            debuffIcon, 
                            enemy.CurrentAction.statusAmount.ToString(), 
                            tooltipDebuffData, 
                            ref entryIndex
                        );
                    }
                    
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