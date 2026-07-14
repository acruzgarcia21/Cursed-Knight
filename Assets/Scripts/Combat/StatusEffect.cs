public class StatusEffect
{
    public StatusType statusType;
    public int amount;
    public int duration;

    public enum StatusType
    {
        Strength,
        Weak,
        Vulnerable,
        Poison,
        Bleed,
        Corruption,
        ViciousResolve,
        DarkMomentum
    }
}
