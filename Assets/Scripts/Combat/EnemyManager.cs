using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    
    // Hardcoded for prototype, will change later
    public int enemiesToSpawn = 4;

    public Transform enemyContainer;
    
    public EnemyData[] enemyStorage;
    
    public List<Transform> enemySpawnPositions = new();
    
    private readonly List<Enemy> _currentEnemies = new();
    
    private BattleManager _battleManager;

    private void Awake()
    {
        if (_battleManager == null)
        {
            _battleManager = FindFirstObjectByType<BattleManager>();
        }
        enemyStorage = Resources.LoadAll<EnemyData>("Enemies");
    }

    public void BattleSetup()
    {
        SpawnEncounter();

        foreach (var enemy in _currentEnemies)
        {
            enemy.InitializeIntent();
        }
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
        _currentEnemies.Add(enemy);
        enemy.BattleSetup();
    }

    public void ProcessEnemyTurn(Player player)
    {
        for (var i = _currentEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = _currentEnemies[i];
 
            if (enemy == null) continue;

            enemy.TakeTurn(player);
        }

        foreach (var enemy in _currentEnemies)
        {
            enemy.SelectNextAction();
        }
    }

    public void RemoveEnemy(Enemy enemyToRemove)
    {
        _currentEnemies.Remove(enemyToRemove);
        Destroy(enemyToRemove.gameObject);
        
        // Okay for now, will change later
        if (AllEnemiesDead())
        {
            BattleManager.Instance.WinBattle();
        }
    }

    public List<Enemy> GetLivingEnemies()
    {
        var livingEnemies = new List<Enemy>(_currentEnemies);
        return livingEnemies;
    }

    public Enemy GetLowestHealthAlly(Enemy healer)
    {
        Enemy healingTarget = null;
        
        var lowestHealthPercentage = 1f;
        var livingEnemies = GetLivingEnemies();

        foreach (var enemy in livingEnemies)
        {
            if (enemy == healer) continue;
            if (enemy.currentEnemyHealth >= enemy.enemyData.enemyMaxHealth) continue;

            var healthPercentage = (float)enemy.currentEnemyHealth / enemy.enemyData.enemyMaxHealth;

            if (healthPercentage >= lowestHealthPercentage) continue;

            lowestHealthPercentage = healthPercentage;
            healingTarget = enemy;
        }
        
        return healingTarget;
    }

    private bool AllEnemiesDead()
    {
        return _currentEnemies.Count == 0;
    }
}