using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }

    public int enemyMaxHealth;
    public int enemyAttackDamage;

    public Sprite enemySprite;
}
