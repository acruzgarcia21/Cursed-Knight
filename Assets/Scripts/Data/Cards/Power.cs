using System;
using CursedKnight;
using UnityEngine;

[CreateAssetMenu(fileName = "New Power Card", menuName = "Card/Power")]
public class Power : Card
{
    [Space(10)] [Header("Status Creation")]
    public StatusDefinition statusToCreate;
}

[Serializable]
public class StatusDefinition
{
    public StatusEffect.StatusType statusType;
    public int amount;
    public int duration;
    
    [Space(10)]
    [Header("UPGRADE ATTRIBUTES")]

    [Header("Upgrade - Status Creation")]
    [SerializeField] private int upgradedStatusAmount;
    [SerializeField] private int upgradedStatusDuration;
    
    public int GetStatusAmount(bool isUpgraded)
    {
        return isUpgraded ? upgradedStatusAmount : amount;
    }

    public int GetStatusDuration(bool isUpgraded)
    {
        return isUpgraded ? upgradedStatusDuration : duration;
    }
}   