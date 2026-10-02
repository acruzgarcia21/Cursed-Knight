using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private RunManager _runManager;
    private DeckManager _deckManager;
    private RelicManager _relicManager;
    private RewardManager _rewardManager;

    private Player _player;

    private string _runSavePath;

    private void Awake()
    {
        _runManager = FindAnyObjectByType<RunManager>();
        _deckManager = FindAnyObjectByType<DeckManager>();
        _relicManager = FindAnyObjectByType<RelicManager>();
        _rewardManager = FindAnyObjectByType<RewardManager>();

        _player = FindAnyObjectByType<Player>();

        _runSavePath = Path.Combine(Application.persistentDataPath, "run_save.json");
    }

    public void SaveRun()
    {
        var runData = CreateRunData();

        var json = JsonUtility.ToJson(runData, true);

        File.WriteAllText(_runSavePath, json);

        Debug.Log($"Run saved to: {_runSavePath}");
    }

    private RunData LoadRunData()
    {
        if (!File.Exists(_runSavePath))
        {
            Debug.LogError("_runSavePath does not exist!");
            return null;
        }

        var json = File.ReadAllText(_runSavePath);
        var runData = JsonUtility.FromJson<RunData>(json);

        return runData;
    }

private RunData CreateRunData()
    {
        var visitedNodeIDs   = new List<string>();
        var acquiredRelicIDs = new List<string>();
        
        var cardSaveDataCollection = new List<CardSaveData>();

        var map = _runManager.GetCurrentMap();
        
        var mapNodeSaveDataCollection = new List<MapNodeSaveData>();
        
        foreach (var runtimeCard in _deckManager.GetPlayerDeck())
        {
            if (runtimeCard == null)
            {
                Debug.LogError("SaveManager: Card is Null!");
                continue;
            }

            var cardSaveData = new CardSaveData(runtimeCard.cardData.GetCardID(), runtimeCard.isUpgraded);
            
            cardSaveDataCollection.Add(cardSaveData);
        }

        foreach (var relic in _relicManager.GetRelicCollection())
        {
            if (relic == null)
            {
                Debug.LogError("SaveManager: Relic is Null!");
                continue;
            }

            var relicID = relic.GetRelicID();
            
            acquiredRelicIDs.Add(relicID);
        }

        foreach (var mapNode in map.GetAllMapNodes())
        {
            if (mapNode == null)
            {
                Debug.LogError("SaveManager: MapNode is Null!");
                continue;
            }

            var mapNodeSaveData = new MapNodeSaveData(mapNode.GetMapNodeID(), mapNode.nodeType);
            
            mapNodeSaveDataCollection.Add(mapNodeSaveData);
        }

        foreach (var visitedNode in _runManager.GetVisitedNodes())
        {
            if (visitedNode == null)
            {
                Debug.LogError("SaveManager: VisitedNode is Null!");
                continue;
            }

            var visitedNodeID = visitedNode.GetMapNodeID();
            
            visitedNodeIDs.Add(visitedNodeID);
        }

        var currentAct = _runManager.GetCurrentAct();
        var currentNodeID = _runManager.GetCurrentNodeID();
        var playerHp = _player.GetPlayerCurrentHealth();
        var playerMaxHp = _player.GetMaxHealth();
        var playerCorruption = _player.GetCorruption();
        var relicsAwardedThisAct = _rewardManager.GetRelicsAwardedThisAct();
        
        
        return new RunData(
            currentAct, 
            currentNodeID, 
            playerHp, 
            playerMaxHp, 
            playerCorruption, 
            relicsAwardedThisAct, 
            visitedNodeIDs, 
            acquiredRelicIDs,
            cardSaveDataCollection,
            mapNodeSaveDataCollection);
    }
}
