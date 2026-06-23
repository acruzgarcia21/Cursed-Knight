using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    
    // Hardcoded for prototype, will change later
    public int enemiesToSpawn = 4;

    public Transform enemyContainer;
    
    public EnemyData[] enemyStorage;
    
    public List<Transform> enemySpawnPositions = new List<Transform>();
    public List<Enemy> currentEnemies = new List<Enemy>();
    
    private BattleManager _battleManager;

    private void Start()
    {
        // Prototype only
        enemyStorage = Resources.LoadAll<EnemyData>("Enemies");
        Debug.Log($"Loaded {enemyStorage.Length} enemies");
        foreach (var enemy in enemyStorage)
        {
            Debug.Log(enemy.enemyName);
        }
    }

    private void Awake()
    {
        if (_battleManager == null)
        {
            _battleManager = FindFirstObjectByType<BattleManager>();
        }
    }

    public void BattleSetup()
    {
        SpawnEncounter();
    }

    private void SpawnEncounter()
    {
        for (var i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy(enemyStorage[i], enemySpawnPositions[i]);
        }
    }

    private void SpawnEnemy(EnemyData enemyData, Transform spawnPositions)
    {
        var enemyObject = Instantiate(enemyPrefab,  spawnPositions.position, spawnPositions.rotation);
        enemyObject.transform.SetParent(enemyContainer, false);
        enemyObject.transform.localPosition = spawnPositions.localPosition;
        enemyObject.transform.localRotation = Quaternion.identity;
        enemyObject.transform.localScale = Vector3.one;
        
        var enemy = enemyObject.GetComponent<Enemy>();
        
        enemy.enemyData = enemyData;
        currentEnemies.Add(enemy);
        enemy.BattleSetup();
    }

    public void ProcessEnemyTurn(Player player)
    {
        foreach (var enemy in currentEnemies)
        {
            enemy.TakeTurn(player);
        }
    }

    // Prototype targeting logic
    public Enemy GetFirstLivingEnemy()
    {
        return currentEnemies.Count > 0 ? currentEnemies[0] : null;
    }

    public void RemoveEnemy(Enemy enemyToRemove)
    {
        currentEnemies.Remove(enemyToRemove);
        Destroy(enemyToRemove.gameObject);
        
        // Okay for now, will change later
        if (AllEnemiesDead())
        {
            BattleManager.Instance.WinBattle();
        }
    }

    public bool AllEnemiesDead()
    {
        return currentEnemies.Count == 0;
    }
}
