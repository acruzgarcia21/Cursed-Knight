using System;
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

            var formattedDamage = hitCount > 1
                ? $"{intentDamage} x {hitCount}"
                : intentDamage.ToString();

            AddIntentEntry(attackIcon, formattedDamage, ref entryIndex);
        }

        if (enemy.CurrentAction.blockAmount > 0)
        {
            AddIntentEntry(blockIcon, enemy.CurrentAction.blockAmount.ToString(), ref entryIndex);
        }
        
        if (enemy.CurrentAction.healingAmount > 0)
        {
            AddIntentEntry(healIcon, enemy.CurrentAction.healingAmount.ToString(), ref entryIndex);
        }

        if (enemy.CurrentAction.appliesStatus && enemy.CurrentAction.statusAmount > 0)
        {
            switch (enemy.CurrentAction.statusTarget)
            {
                case EnemyActionData.StatusTargetType.Self:
                    AddIntentEntry(buffIcon, enemy.CurrentAction.statusAmount.ToString(), ref entryIndex);
                    break;
                case EnemyActionData.StatusTargetType.Player:
                    AddIntentEntry(debuffIcon, enemy.CurrentAction.statusAmount.ToString(), ref entryIndex);
                    break;
            }
        }
    }
    
    private void AddIntentEntry(Sprite icon, string text, ref int entryIndex)
    {
        if (icon == null || intentEntries == null || entryIndex >= intentEntries.Length)
        {
            return;
        }

        intentEntries[entryIndex].DisplayIntent(icon, text);
        entryIndex++;
    }
}