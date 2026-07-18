using UnityEngine;

[CreateAssetMenu(fileName = "New Status Display Data", menuName = "Status/Display Data")]
public class StatusDisplayData : ScriptableObject
{
    public StatusEffect.StatusType statusType;
    public Sprite icon;
    public StatusCountDisplayType countDisplayType;
    public string displayName;
    public string description;

    public enum StatusCountDisplayType
    {
        Amount,
        Duration,
    }
}
