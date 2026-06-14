using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    public int currentEnemyHealth;
    public int currentEnemyBlock;

    [SerializeField] public EnemyData enemyData;

    private void Awake()
    {
        BattleSetup();
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
    }

    public bool EnemyIsDead()
    {
        return currentEnemyHealth <= 0;
    }

    private void BattleSetup()
    {
        currentEnemyHealth = enemyData.enemyMaxHealth;
        currentEnemyBlock = 0;
    }
}
