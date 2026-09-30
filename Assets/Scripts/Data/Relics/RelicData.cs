using UnityEngine;

[CreateAssetMenu(fileName = "New Relic", menuName = "Relic")]
public abstract class RelicData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string relicName;
    [SerializeField] private string relicDescription;
    [SerializeField] private string relicID;

    [SerializeField] private Sprite relicSprite;

    [Space(10)] [Header("Relic Info")] 
    [SerializeField] private TriggerTime triggerTime;
    
    [SerializeField] private RelicType relicType;

    [SerializeField] private TargetType targetType;
    
    [Space(10)] [Header("Relic Cost")] 
    [SerializeField] protected int corruptionCost;

    public enum TriggerTime
    {
        StartOfCombat,
        StartOfTurn,
        EndOfCombat,
        CorruptionOverflow,
        CriticalHealth,
        None
    }
    
    public enum RelicType
    {
        Attack,
        Defense,
        Utility
    }
    
    public enum TargetType
    {
        SingleEnemy,
        AllEnemies,
        RandomEnemy,
        Self,
        None
    }

    public string GetRelicID()
    {
        return relicID;
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(relicID)) return;

        relicID = GenerateId(name);
    }

    private string GenerateId(string assetName)
    {
        if (string.IsNullOrWhiteSpace(assetName)) return string.Empty;

        var id = assetName.Trim().ToLowerInvariant();

        id = id.Replace("'", "");
        id = id.Replace("’", "");
        id = id.Replace("-", "_");
        id = id.Replace(" ", "_");

        while (id.Contains("__"))
        {
            id = id.Replace("__", "_");
        }

        return id;
    }
    #endif

    public RelicType GetRelicType()
    {
        return relicType;
    }

    public TriggerTime GetTriggerTime()
    {
        return triggerTime;
    }

    public string GetRelicName()
    {
        return relicName;
    }

    public string GetRelicDescription()
    {
        return relicDescription;
    }

    public abstract void ResolveEffect(Player player, EnemyManager enemyManager);
}
