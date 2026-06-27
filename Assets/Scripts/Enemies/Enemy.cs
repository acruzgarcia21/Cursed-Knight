using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int currentEnemyHealth;
    public int currentEnemyBlock;

    public EnemyData enemyData;

    private EnemyDisplay _enemyDisplay;

    private void Awake()
    {
        _enemyDisplay = GetComponent<EnemyDisplay>();
    }

    public void BattleSetup()
    {
        currentEnemyHealth = enemyData.enemyMaxHealth;
        currentEnemyBlock = 0;

        _enemyDisplay.UpdateEnemyDisplay();
    }

    public void TakeTurn(Player player)
    {
        player.TakeDamage(enemyData.enemyAttackDamage);
    }

    public void TakeDamage(int damage)
    {
        if (currentEnemyBlock > 0)
        {
            if (currentEnemyBlock >= damage)
            {
                currentEnemyBlock -= damage;
            }
            else
            {
                damage -= currentEnemyBlock;
                currentEnemyBlock = 0;
                currentEnemyHealth -= damage;
            }
        }
        else
        {
            currentEnemyHealth -= damage;
        }

        currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, enemyData.enemyMaxHealth);

        _enemyDisplay.UpdateEnemyDisplay();

        if (EnemyIsDead())
        {
            BattleManager.Instance.EnemyManager.RemoveEnemy(this);
        }
    }

    public bool EnemyIsDead()
    {
        return currentEnemyHealth <= 0;
    }
}