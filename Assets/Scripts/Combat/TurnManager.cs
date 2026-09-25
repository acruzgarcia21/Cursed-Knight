using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int targetHandSize = 6;
    
    public enum TurnState { Player, Enemy }
    public TurnState currentState;
    
    private Player _player;
    private EnemyManager _enemyManager;
    private HandManager _handManager;

    private void Awake()
    {
        _player       = FindAnyObjectByType<Player>();
        _enemyManager = FindAnyObjectByType<EnemyManager>();
        _handManager  = FindAnyObjectByType<HandManager>();
    }

    public void EndTurn()
    {
        if (currentState != TurnState.Player) return;
        
        PlayerEndTurn();
        EnemyTurn();
        StartPlayerTurn();
    }

    public void StartCombat()
    {
        currentState = TurnState.Player;
        _player.StartCombat();
        _handManager.PrepareHandForTurn(targetHandSize);
        Debug.Log("Start of Combat, player's turn");
    }
    private void StartPlayerTurn()
    {
        currentState = TurnState.Player;
        _player.StartTurn();
        _handManager.PrepareHandForTurn(targetHandSize);
        Debug.Log("Now player turn");
    }

    private void PlayerEndTurn()
    {
        if (currentState != TurnState.Player) return;
        
        _player.EndTurn();
        _handManager.DiscardHand();
        currentState = TurnState.Enemy;
        Debug.Log("Player turn ended, now enemy turn");
    }

    private void EnemyTurn()
    {
        _enemyManager.ProcessEnemyTurn(_player);
    }

}
