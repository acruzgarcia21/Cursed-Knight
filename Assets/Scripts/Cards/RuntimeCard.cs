using CursedKnight;

public class RuntimeCard
{
    public Card cardData;
    
    public bool retain;
    public bool exhaust;
    public bool spectral;
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
