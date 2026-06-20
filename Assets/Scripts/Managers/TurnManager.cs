using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState { Player, Enemy }
    public TurnState currentState;
    
    private Player _player;
    private EnemyManager _enemyManager;

    private void Awake()
    {
        _player       = FindFirstObjectByType<Player>();
        _enemyManager = FindFirstObjectByType<EnemyManager>();
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
            currentState = TurnState.Enemy;
            Debug.Log("Player turn ended, now enemy turn");
        }
    }
    
    public void EnemyTurn()
    {
        foreach (var enemy in _enemyManager.currentEnemies)
        {
            _player.TakeDamage(enemy.enemyData.enemyAttackDamage);
        }
    }

    public void StartPlayerTurn()
    {
        currentState = TurnState.Player;
        _player.StartTurn();
        Debug.Log("Now player turn");
    }
}
