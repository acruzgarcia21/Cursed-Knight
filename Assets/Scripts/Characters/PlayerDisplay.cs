using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    // All Card Elements
    public Player player;
    
    public TMP_Text playerHealthText;
    public TMP_Text playerBlockText;
    
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
        playerBlockText.text = player.playerBlock.ToString();
    }
}
