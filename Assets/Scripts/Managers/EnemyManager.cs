using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemiesToSpawn;
    
    public List<Transform> enemySpawnPositions = new List<Transform>();
    
    public List<Enemy> currentEnemies = new List<Enemy>();
    
    private BattleManager _battleManager;

    private void Start()
    {
        var enemies = Resources.LoadAll<EnemyData>("Enemies");
    }

    private void Awake()
    {
        if (_battleManager == null)
        {
            _battleManager = FindFirstObjectByType<BattleManager>();
        }
    }

    private void BattleSetup()
    {
        
    }

    private void SpawnEncounter()
    {
        
    }

    private void SpawnEnemy(EnemyData enemy, Transform spawnPositions)
    {
        
    }
}
