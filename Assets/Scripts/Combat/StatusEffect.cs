public class StatusEffect
{
    public StatusType statusType;
    
    public int amount;
    public int duration;
    
    public bool hasTriggered = false;

    public StatusEffect statusToCreate;
    public enum StatusType
    {
        Strength,
        Weak,
        Vulnerable,
        Poison,
        Bleed,
        Corruption,
        ViciousResolve,
        DarkMomentum,
        DarkCommunion,
        CorruptedSoul,
        BloodMoon,
        BloodCurse
    }
}
