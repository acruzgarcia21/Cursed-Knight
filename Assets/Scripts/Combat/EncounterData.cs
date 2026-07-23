using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Encounter")]
public class EncounterData : ScriptableObject
{
    public List<EnemyData> enemies = new();
}
