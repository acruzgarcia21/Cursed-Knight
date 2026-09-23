using CursedKnight;

public class RuntimeCard
{
    public readonly Card cardData;
    
    public readonly bool retain;
    public readonly bool exhaust;
    public readonly bool spectral;
    public bool createdDuringCombat;

    public RuntimeCard(Card cardData, bool createdDuringCombat = false)
    {
        if (cardData == null) return; 
        this.cardData            = cardData;
        retain                   = cardData.retain;
        exhaust                  = cardData.exhaust;
        spectral                 = cardData.spectral;
        this.createdDuringCombat = createdDuringCombat;
    }
}
