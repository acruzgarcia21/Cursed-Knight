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
        _player          = FindFirstObjectByType<Player>();
        _enemyManager    = FindFirstObjectByType<EnemyManager>();
        _handManager     = FindFirstObjectByType<HandManager>();
    }

    public void EndTurn()
    {
        if (currentState != TurnState.Player) return;
        
        PlayerEndTurn();
        EnemyTurn();
        StartPlayerTurn();
    }

    public void PlayerEndTurn()
    {
        if (currentState == TurnState.Player)
        {
            _player.EndTurn();
            _handManager.DiscardHand();
            currentState = TurnState.Enemy;
            Debug.Log("Player turn ended, now enemy turn");
        }
    }
    
    public void EnemyTurn()
    {
        _enemyManager.ProcessEnemyTurn(_player);
    }

    public void StartPlayerTurn()
    {
        currentState = TurnState.Player;
        _player.StartTurn();
        _handManager.PrepareHandForTurn(targetHandSize);
        Debug.Log("Now player turn");
    }
}
