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
    public Image playerHealthBarFill;

    [SerializeField] private Color healthColor;
    [SerializeField] private Color blockColor;

    [SerializeField] private GameObject blockUI;

    public void Awake()
    {
        player = GetComponent<Player>();
        
        blockUI.SetActive(false);
    }

    private void Start()
    {
        UpdatePlayerDisplay();
    }
    // Updates all card data populated by each card in player's hand/deck
    public void UpdatePlayerDisplay()
    {
        playerHealthText.text = player.playerHealth + "/" + player.playerMaxHealth;
        
        var healthPercent = (float)player.playerHealth / player.playerMaxHealth;
        playerHealthBarFill.fillAmount = healthPercent;
        
        playerBlockText.text = player.playerBlock.ToString();
        
        if (player.playerBlock > 0)
        {
            blockUI.SetActive(true);
            playerHealthBarFill.color = blockColor;
        }
        else
        {
            blockUI.SetActive(false);
            playerHealthBarFill.color = healthColor;
        }
    }
}
