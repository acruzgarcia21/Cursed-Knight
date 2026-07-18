using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    public Enemy enemy;

    public TMP_Text enemyName;
    public TMP_Text enemyHealth;
    public TMP_Text enemyAttackDamage;

    public Image enemySprite;

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

        enemyName.text = enemy.enemyData.enemyName;
        enemyHealth.text = enemy.currentEnemyHealth + " / " + enemy.enemyData.enemyMaxHealth;
        enemySprite.sprite = enemy.enemyData.enemySprite;

        UpdateIntentDisplay();
    }

    private void UpdateIntentDisplay()
    {
        if (enemy.CurrentAction == null)
        {
            enemyAttackDamage.text = string.Empty;
            return;
        }

        var intentDamage = enemy.GetCurrentIntentDamage();

        if (intentDamage <= 0)
        {
            enemyAttackDamage.text = string.Empty;
            return;
        }

        var hitCount = Mathf.Max(1, enemy.CurrentAction.hitCount);

        enemyAttackDamage.text = hitCount > 1
            ? $"{intentDamage} x {hitCount}"
            : intentDamage.ToString();
    }
}