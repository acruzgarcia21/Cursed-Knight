using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int targetHandSize = 6;

    [SerializeField] private GameObject endTurnButton;
    
    public enum TurnState { Player, Enemy }
    private TurnState _currentState;
    
    private Player _player;
    private EnemyManager _enemyManager;
    private HandManager _handManager;
    private AudioManager _audioManager;
    private CardPlayManager _cardPlayManager;

    private bool _isResolvingTurn;
    private bool _combatActive;

    private void Awake()
    {
        _player       = FindAnyObjectByType<Player>();
        _enemyManager = FindAnyObjectByType<EnemyManager>();
        _handManager  = FindAnyObjectByType<HandManager>();
        _audioManager = FindAnyObjectByType<AudioManager>();
        _cardPlayManager = FindAnyObjectByType<CardPlayManager>();
    }

    public void EndTurn()
    {
        if (!_combatActive || _cardPlayManager.IsResolvingCard()) return;
        if (_currentState != TurnState.Player || _isResolvingTurn) return;

        _isResolvingTurn = true;
        _audioManager.PlayEndTurnSound();
        StartCoroutine(ResolveTurn());
    }

    public void StartCombat()
    {
        endTurnButton.SetActive(true);

        _combatActive = true;
        _isResolvingTurn = false;
        
        _currentState = TurnState.Player;
        
        _player.StartCombat();
        
        _handManager.PrepareHandForTurn(targetHandSize);
        
        _audioManager.PlayStartTurnSound();
        
        Debug.Log("Start of Combat, player's turn");
    }

    public void EndCombat()
    {
        _combatActive = false;
        
        _player.EndCombat();
    }

    public bool IsResolvingTurn()
    {
        return _isResolvingTurn;
    }

    public bool IsCombatActive()
    {
        return _combatActive;
    }

    public TurnState GetTurnState()
    {
        return _currentState;
    }
    private void StartPlayerTurn()
    {
        _currentState = TurnState.Player;
        
        endTurnButton.SetActive(true);
        
        _player.StartTurn();
        
        _handManager.PrepareHandForTurn(targetHandSize);
        
        _audioManager.PlayStartTurnSound();
        
        Debug.Log("Now player turn");
    }

    private IEnumerator PlayerEndTurn()
    {
        if (_currentState != TurnState.Player) yield break;
        
        yield return _player.EndTurn();
        
        _handManager.DiscardHand();
        
        _currentState = TurnState.Enemy;
        
        endTurnButton.SetActive(false);
        
        Debug.Log("Player turn ended, now enemy turn");
    }
    
    private IEnumerator ResolveTurn()
    {
        yield return PlayerEndTurn();

        if (!IsCombatActive() || _player.IsDead())
        {
            _isResolvingTurn = false;
            yield break;
        }

        yield return EnemyTurn();

        if (!IsCombatActive() || _player.IsDead())
        {
            _isResolvingTurn = false;
            yield break;
        }
        
        StartPlayerTurn();

        _isResolvingTurn = false;
    }

    private IEnumerator EnemyTurn()
    {
        yield return _enemyManager.ProcessEnemyTurn(_player);
    }

}
