using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance  { get; private set; }
    public EnemyManager EnemyManager { get; private set; }
    
    private TurnManager _turnManager;
    private DeckManager _deckManager;
    private RewardManager _rewardManager;
    
    private Player _player;
    
    private void Awake()
    {
        Instance = this;
        EnemyManager = GetComponentInChildren<EnemyManager>();

        if (EnemyManager == null)
        {
            Debug.Log("EnemyManager not found under BattleManager");
        }

        _turnManager   = FindAnyObjectByType<TurnManager>();
        _deckManager   = FindAnyObjectByType<DeckManager>();
        _rewardManager = FindAnyObjectByType<RewardManager>();
        _player        = FindAnyObjectByType<Player>();
    }

    public void StartBattle(EncounterData encounter)
    {
        if (encounter == null) return;
        
        _player.BattleSetup();
        
        _deckManager.BattleSetup();
        EnemyManager.BattleSetup(encounter);
        _turnManager.StartCombat();
    }

    public void WinBattle()
    {
        Debug.Log("Battle won");
        _rewardManager.StartRewards();
        Debug.Log("Reached Start Rewards");
    }

    public void LoseBattle()
    {
        Debug.Log("Battle lost");
    }
}
