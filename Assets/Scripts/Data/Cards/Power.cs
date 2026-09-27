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
}