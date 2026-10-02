using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private RunManager _runManager;
    private DeckManager _deckManager;
    private RelicManager _relicManager;
    private RewardManager _rewardManager;
    private OptionsManager _optionsManager;

    private Player _player;

    private string _runSavePath;
    private string _settingsSavePath;

    private bool _canSaveRun;

    private void Awake()
    {
        _runManager     = FindAnyObjectByType<RunManager>();
        _deckManager    = FindAnyObjectByType<DeckManager>();
        _relicManager   = FindAnyObjectByType<RelicManager>();
        _rewardManager  = FindAnyObjectByType<RewardManager>();
        _optionsManager = FindAnyObjectByType<OptionsManager>();
        
        _player = FindAnyObjectByType<Player>();

        _runSavePath      = Path.Combine(Application.persistentDataPath, "run_save.json");
        _settingsSavePath = Path.Combine(Application.persistentDataPath, "settings.json");
    }

    private void Start()
    {
        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _runManager.StartNewRun();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            _optionsManager.SetMutingAudio(!_optionsManager.GetMutingAudio());
            SaveSettings();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveRun();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadRun();
        }
    }
    
    public void SaveCheckpoint()
    {
        EnableRunSaving();
        SaveRun();
    }
    
    public bool HasRunSave()
    {
        return File.Exists(_runSavePath);
    }

    public void DeleteRunSave()
    {
        if (!File.Exists(_runSavePath))
        {
            return;
        }

        File.Delete(_runSavePath);

        Debug.Log("SaveManager: Run save deleted.");
    }
    
    public void EnableRunSaving()
    {
        _canSaveRun = true;
    }

    public void DisableRunSaving()
    {
        _canSaveRun = false;
    }

    public bool CanSaveRun()
    {
        return _canSaveRun;
    }

    public void SaveRun()
    {
        if (!_canSaveRun)
        {
            Debug.LogWarning("SaveManager: Cannot save run outside of a valid checkpoint!");
            return;
        }

        var runData = CreateRunData();

        var json = JsonUtility.ToJson(runData, true);

        File.WriteAllText(_runSavePath, json);

        Debug.Log($"Run saved to: {_runSavePath}");
    }

    public void LoadRun()
    {
        if (!HasRunSave())
        {
            Debug.LogWarning("SaveManager: No run save exists!");
            return;
        }
        
        var runData = LoadRunData();

        if (runData == null)
        {
            Debug.LogError("SaveManager: Run data is null! Unable to load run");
            return;
        }

        var currentMap = _runManager.GetCurrentMap();

        if (currentMap == null)
        {
            Debug.LogError("SaveManager: Current map is null! Unable to load run");
            return;
        }
        
        currentMap.RestoreMapNodeTypes(runData.GetMapNodeSaveDataCollection());
        
        _runManager.RestoreRun(
            runData.GetCurrentAct(), 
            runData.GetCurrentNodeID(), 
            runData.GetVisitedNodeIDs());
        
        _player.RestoreRunState(
            runData.GetPlayerHp(), 
            runData.GetPlayerMaxHp(), 
            runData.GetPlayerCorruption());
        
        _deckManager.RestorePlayerDeck(runData.GetCardSaveDataCollection());
        
        _relicManager.RestoreRelicsToRelicCollection(
            runData.GetCurrentAct(), 
            runData.GetAcquiredRelicIDs());
        
        _rewardManager.RestoreRelicsRewardedThisAct(runData.GetRelicsAwardedThisAct());
        
        _runManager.FinishRunRestoration();
        
        EnableRunSaving();
    }
    
    public void SaveSettings()
    {
        var settingsData = new SettingsData(_optionsManager.GetMutingAudio());

        var json = JsonUtility.ToJson(settingsData, true);

        File.WriteAllText(_settingsSavePath, json);

        Debug.Log($"Settings saved to: {_settingsSavePath}");
    }

    public void LoadSettings()
    {
        if (!File.Exists(_settingsSavePath))
        {
            return;
        }

        var json = File.ReadAllText(_settingsSavePath);

        var settingsData = JsonUtility.FromJson<SettingsData>(json);

        if (settingsData == null)
        {
            Debug.LogWarning("SaveManager: Failed to load settings.");
            return;
        }

        _optionsManager.RestoreSettings(settingsData);
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
