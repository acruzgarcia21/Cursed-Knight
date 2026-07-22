using System;
using System.Collections.Generic;
using UnityEngine;

// Stores immutable enemy data
[CreateAssetMenu(fileName = "New Enemy", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("General")] 
    public string enemyName;
    public Sprite enemySprite;
    public EnemyType enemyType;
    public ActionSelectionType actionSelectionType;
    
    [Space(10)] [Header("Enemy Stats")]
    public int enemyMaxHealth;
    
    [Space(10)] [Header("Enemy Actions")] 
    public List<EnemyActionData> enemyActions = new();

    
    [Space(10)] [Header("Health Phases")]
    public List<EnemyPhaseData> healthPhases = new();
    
    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }

    public enum ActionSelectionType
    {
        FixedPattern,
        WeightedRandom
    }
}

// Stores one action definition
[Serializable]
public class EnemyActionData
{
    [Header("General")]
    public string actionName;

    [Space(10)] [Header("Damage")]
    public int damage;
    public int hitCount = 1;

    [Space(10)] [Header("Defense")]
    public int blockAmount;
    public int healingAmount;

    [Space(10)] [Header("Status Effect")]
    public bool appliesStatus;
    public StatusEffect.StatusType statusType;
    public int statusAmount;
    public int statusDuration;
    public StatusTargetType statusTarget;

    [Space(10)] [Header("Weighted Selection")]
    public int selectionWeight = 1;
    public int maximumConsecutiveUses = 1;

    [Space(10)] [Header("Temporary Buffs")]
    public int nextAttackBonusDamage;

    [Space(10)] [Header("Hides Enemy")]
    public bool hidesEnemy;
    
    public enum StatusTargetType
    {
        Self,
        Player,
        AllOtherAllies,
        RandomAlly
    }
}

[Serializable]
public class EnemyPhaseData
{
    [Header("Trigger")]
    public int healthThreshold;

    [Header("Status Effect")]
    public bool appliesStatus;
    public StatusEffect.StatusType statusType;
    public int statusAmount;
    public int statusDuration = -1;
}
