using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private ActEnvironmentDisplay actEnvironmentDisplay;    
    
    public static BattleManager Instance  { get; private set; }
    public EnemyManager EnemyManager { get; private set; }
    
    private TurnManager _turnManager;
    private DeckManager _deckManager;
    private RewardManager _rewardManager;
    private AudioManager _audioManager;
    private RunManager _runManager;
    
    private Player _player;

    private Map.MapNodeType _currentNodeType;

    private bool isFinalStage;
    
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
        _audioManager  = FindAnyObjectByType<AudioManager>();
        _runManager    = FindAnyObjectByType<RunManager>();
        _player        = FindAnyObjectByType<Player>();
    }

    public void StartBattle(EncounterData encounter, Map.MapNodeType currentNodeType, bool stageComparison)
    {
        if (encounter == null) return;

        _currentNodeType = currentNodeType;

        isFinalStage = stageComparison;
        
        actEnvironmentDisplay.UpdateEnvironment(_runManager.GetCurrentAct(), _currentNodeType);
        
        _audioManager.StopBattleMusic();

        switch (_runManager.GetCurrentAct())
        {
            case RunManager.CurrentAct.ActOne:
                if (_currentNodeType == Map.MapNodeType.Boss) _audioManager.PlayBossBattleMusic();
                else _audioManager.PlayActOneBattleMusic();
                break;
            case RunManager.CurrentAct.ActTwo:
                if (_currentNodeType == Map.MapNodeType.Boss) _audioManager.PlayBossBattleMusic();
                else _audioManager.PlayActTwoBattleMusic();
                break;
            case RunManager.CurrentAct.ActThree:
                if (_currentNodeType == Map.MapNodeType.Boss) _audioManager.PlayFinalBossBattleMusic();
                else _audioManager.PlayActThreeBattleMusic();
                break;
        }

        _player.BattleSetup();
        
        _deckManager.BattleSetup();
        EnemyManager.BattleSetup(encounter);
        _turnManager.StartCombat();
    }

    public void WinBattle()
    {
        _audioManager.PlayVictoryMusic();
        
        Debug.Log("Battle won");
        
        _turnManager.EndCombat();
        
        _rewardManager.StartRewards(_currentNodeType, isFinalStage);
    }

    public void LoseBattle()
    {
        _audioManager.StopBattleMusic();
        
        _turnManager.EndCombat();
        
        Debug.Log("Battle lost");
    }
}
