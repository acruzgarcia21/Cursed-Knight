using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int startingHandSize = 6;
    
    public enum TurnState { Player, Enemy }
    public TurnState currentState;
    
    private Player _player;
    private EnemyManager _enemyManager;
    private HandManager _handManager;
    private DrawPileManager _drawPileManager;
    private UIManager _uiManager;

    private void Awake()
    {
        _player          = FindFirstObjectByType<Player>();
        _enemyManager    = FindFirstObjectByType<EnemyManager>();
        _handManager     = FindFirstObjectByType<HandManager>();
        _drawPileManager = FindFirstObjectByType<DrawPileManager>();
        _uiManager       = FindFirstObjectByType<UIManager>();
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
        foreach (var enemy in _enemyManager.currentEnemies)
        {
            _player.TakeDamage(enemy.enemyData.enemyAttackDamage);
        }
    }

    public void StartPlayerTurn()
    {
        currentState = TurnState.Player;
        _player.StartTurn();
        _drawPileManager.DrawCards(startingHandSize);
        _uiManager.UpdatePlayerEnergyText();
        _uiManager.UpdatePlayerCorruptionText();
        Debug.Log("Now player turn");
    }
}
