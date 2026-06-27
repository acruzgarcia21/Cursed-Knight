using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance  { get; private set; }
    public EnemyManager EnemyManager { get; private set; }
    private TurnManager _turnManager;
    private DeckManager _deckManager;
    private Player _player;
    
    private void Awake()
    {
        Instance = this;
        EnemyManager = GetComponentInChildren<EnemyManager>();

        if (EnemyManager == null)
        {
            Debug.Log("EnemyManager not found under BattleManager");
        }

        _turnManager = FindFirstObjectByType<TurnManager>();
        _deckManager = FindFirstObjectByType<DeckManager>();
        _player      = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        StartBattle();
    }

    private void StartBattle()
    {
        _player.BattleSetup();
        
        _deckManager.BattleSetup();
        EnemyManager.BattleSetup();
        _turnManager.StartPlayerTurn();
    }

    public void WinBattle()
    {
        Debug.Log("Battle won");
    }

    public void LoseBattle()
    {
        Debug.Log("Battle lost");
    }
}
