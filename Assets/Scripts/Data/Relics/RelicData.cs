using UnityEngine;

[CreateAssetMenu(fileName = "New Relic", menuName = "Relic")]
public class RelicData : ScriptableObject
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
        EndOfCombat
    }
    
    public enum RelicType
    {
        Attack,
        Defense,
        Utility,
        Power
    }
    
    public enum TargetType
    {
        SingleEnemy,
        AllEnemies,
        RandomEnemy,
        Self,
        None
    }
}
