using UnityEngine;

[CreateAssetMenu(fileName = "New Relic", menuName = "Relic")]
public abstract class RelicData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string relicName;
    [SerializeField] private string relicDescription;

    [SerializeField] private Sprite relicSprite;

    [Space(10)] [Header("Relic Info")] 
    [SerializeField] private TriggerTime triggerTime;
    
    [SerializeField] private RelicType relicType;

    [SerializeField] private TargetType targetType;

    public enum TriggerTime
    {
        StartOfCombat,
        EndOfCombat,
        CorruptionOverflow,
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

    public RelicType GetRelicType()
    {
        return relicType;
    }

    public TriggerTime GetTriggerTime()
    {
        return triggerTime;
    }

    public abstract void ResolveEffect(Player player);
}
