using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    // All Card Elements
    public Enemy enemy;
    
    public TMP_Text enemyName;
    public TMP_Text enemyHealth;
    public TMP_Text enemyAttackDamage;
    
    public Image enemySprite; 

    private void Start()
    {
        UpdateEnemyDisplay();
    }
    // Updates all card data populated by each card in player's hand/deck
    public void UpdateEnemyDisplay()
    {
        enemyName.text = enemy.enemyData.enemyName;
        enemyHealth.text = enemy.currentEnemyHealth + " / " + enemy.enemyData.enemyMaxHealth;
        enemyAttackDamage.text = enemy.enemyData.enemyAttackDamage.ToString();
        // enemySprite = enemyData.enemySprite;
    }
}
