using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    // All Card Elements
    public Player player;
    
    public TMP_Text playerHealthText;
    public TMP_Text playerEnergyText;
    public TMP_Text playerBlockText;
    public TMP_Text playerCorruptionText;
    public TMP_Text playerCorruptionRoundsText;
    
    public Image playerSprite;

    public void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        UpdatePlayerDisplay();
    }
    // Updates all card data populated by each card in player's hand/deck
    public void UpdatePlayerDisplay()
    {
        playerHealthText.text = player.playerHealth + "/" + player.playerMaxHealth;
        //playerEnergyText.text = player.playerEnergy.ToString() + "/" + player.playerMaxEnergy;
        playerBlockText.text = player.playerBlock.ToString();
        // playerCorruptionText.text = player.playerCorruption + " / " + player.playerMaxCorruption;
        // playerCorruptionRoundsText.text = player.corruptionDebuffTurns.ToString();
    }
}
