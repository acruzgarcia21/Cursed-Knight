using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("General")] 
    public string enemyName;
    public Sprite enemySprite;

    [Space(10)] [Header("Enemy Info")]
    public int enemyMaxHealth;
    public int enemyAttackDamage;
    
    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }

    [Space(10)] [Header("Status Effects")]
    public bool appliesStatus;
    public StatusEffect.StatusType statusType;
    public int statusAmount;
    public int statusDuration;
}
