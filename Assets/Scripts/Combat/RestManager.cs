using UnityEngine;

public class RestManager : MonoBehaviour
{
    public event System.Action OnRestCompleted;
    
    private Player _player;
    
    private const float RestHealScale = 0.30f;
    
    private CardRemovalManager _cardRemovalManager;
    private CardUpgradeManager _cardUpgradeManager;

    [SerializeField] private RestScreenDisplay restScreenDisplay;

    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        
        _cardRemovalManager = FindAnyObjectByType<CardRemovalManager>();
        _cardUpgradeManager = FindAnyObjectByType<CardUpgradeManager>();
    }

    public void OnHealSelection()
    {
        if (_player == null) return;

        var playerMaxHealth = _player.GetMaxHealth();
        var healthToGain = Mathf.CeilToInt(playerMaxHealth * RestHealScale);
        
        _player.Heal(healthToGain);
        
        OnRestCompleted?.Invoke();
    }

    private void OnEnable()
    {
        if (_cardRemovalManager != null)
            _cardRemovalManager.OnCardRemoved += HandleCardManipulation;

        if (_cardUpgradeManager != null)
            _cardUpgradeManager.OnCardUpgraded += HandleCardManipulation;
    }

    private void OnDisable()
    {
        if (_cardRemovalManager != null)
            _cardRemovalManager.OnCardRemoved -= HandleCardManipulation;

        if (_cardUpgradeManager != null)
            _cardUpgradeManager.OnCardUpgraded -= HandleCardManipulation;
    }

    private void HandleCardManipulation()
    {
        OnRestCompleted?.Invoke();
    }
    
    public void StartRestNode()
    {
        restScreenDisplay.DisplayRestScreen();
    }
}
