using UnityEngine;

public class RestManager : MonoBehaviour
{
    public event System.Action OnRestCompleted;
    
    private Player _player;
    
    private const float RestHealScale = 0.30f;
    
    private CardRemovalManager _cardRemovalManager;

    [SerializeField] private RestScreenDisplay restScreenDisplay;

    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        _cardRemovalManager = FindAnyObjectByType<CardRemovalManager>();
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
        _cardRemovalManager.OnCardRemoved += HandleCardRemoved;
    }

    private void OnDisable()
    {
        _cardRemovalManager.OnCardRemoved -= HandleCardRemoved;
    }

    private void HandleCardRemoved()
    {
        OnRestCompleted?.Invoke();
    }

    public void StartRestNode()
    {
        restScreenDisplay.DisplayRestScreen();
    }
}
