using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Transform enemyContainer;
    
    
    public List<Transform> enemySpawnPositions = new();
    
    private readonly List<Enemy> _currentEnemies = new();
    
    private BattleManager _battleManager;

    [SerializeField] private EncounterData encounter;

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

        foreach (var enemy in _currentEnemies)
        {
            enemy.InitializeIntent();
        }
    }

    private void SpawnEncounter()
    {
        if (encounter == null) return;

        var actualEnemyCount = Mathf.Min(encounter.enemies.Count, enemySpawnPositions.Count);

        for (var i = 0; i < actualEnemyCount; i++)
        {
            SpawnEnemy(encounter.enemies[i], enemySpawnPositions[i]);
        }
    }

    private Enemy SpawnEnemy(EnemyData enemyData, Transform spawnPosition)
    {
        var enemyObject = Instantiate(enemyPrefab,  spawnPosition.position, spawnPosition.rotation);
        enemyObject.transform.SetParent(enemyContainer, false);
        enemyObject.transform.localPosition = spawnPosition.localPosition;
        enemyObject.transform.localRotation = Quaternion.identity;
        enemyObject.transform.localScale = Vector3.one;
        
        var enemy = enemyObject.GetComponent<Enemy>();
        
        enemy.enemyData = enemyData;
        enemy.spawnPoint = spawnPosition;
        
        _currentEnemies.Add(enemy);
        enemy.BattleSetup();

        return enemy;
    }

    public void SummonEnemies(EnemyData enemyToSummon, int summonCount)
    {
        var availableSlots = GetAvailableSpawnSlots();
        var actualSummons = Mathf.Min(summonCount, availableSlots.Count);
        
        Debug.Log(
            $"Summon requested: {summonCount} | " +
            $"Available slots: {availableSlots.Count} | " +
            $"Actual summons: {actualSummons}"
        );

        for (var i = 0; i < actualSummons; i++)
        { 
            var enemy = SpawnEnemy(enemyToSummon, availableSlots[i]);
            enemy.InitializeIntent();
        }
    }

    public void ProcessEnemyTurn(Player player)
    {
        var currentEnemies = GetLivingEnemies();
        
        foreach (var enemy in currentEnemies)
        {
            if (enemy == null) continue;

            enemy.TakeTurn(player);
        }

        foreach (var enemy in currentEnemies)
        {
            if (!_currentEnemies.Contains(enemy)) continue;
            
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
    
    public bool HasAvailableSpawnSlot()
    {
        return GetAvailableSpawnSlots().Count > 0;
    }

    private List<Transform> GetAvailableSpawnSlots()
    {
        var availableSpawnSlots = new List<Transform>();
        var currentLivingEnemies = GetLivingEnemies();

        foreach (var position in enemySpawnPositions)
        {
            var isOccupied = false;
            
            foreach (var enemy in currentLivingEnemies)
            {
                if (enemy.spawnPoint != position) continue;

                isOccupied = true;
                break;
            }

            if (!isOccupied)
            {
                availableSpawnSlots.Add(position);
            }
        }

        return availableSpawnSlots;
    }

    private bool AllEnemiesDead()
    {
        return _currentEnemies.Count == 0;
    }
}