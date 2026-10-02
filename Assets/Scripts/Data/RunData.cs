using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunData
{
    [SerializeField] private RunManager.CurrentAct currentAct;

    [SerializeField] private string currentNodeID;
    
    [SerializeField] private int playerHp;

    [SerializeField] private int playerMaxHp;

    [SerializeField] private int playerCorruption;
    
    [SerializeField] private int relicsAwardedThisAct;
    
    [SerializeField] private List<string> visitedNodeIDs = new();
    
    [SerializeField] private List<string> acquiredRelicIDs = new();

    [SerializeField] private List<CardSaveData> cardSaveDataCollection = new();

    [SerializeField] private List<MapNodeSaveData> mapNodeSaveDataCollection = new();
    
    public RunData(
        RunManager.CurrentAct currentAct,
        string currentNodeID,
        int playerHp,
        int playerMaxHp,
        int playerCorruption,
        int relicsAwardedThisAct,
        List<string> visitedNodeIDs,
        List<string> acquiredRelicIDs,
        List<CardSaveData> cardSaveDataCollection,
        List<MapNodeSaveData> mapNodeSaveDataCollection)
    {
        this.currentAct                = currentAct;
        this.currentNodeID             = currentNodeID;
        this.playerHp                  = playerHp;
        this.playerMaxHp               = playerMaxHp;
        this.playerCorruption          = playerCorruption;
        this.relicsAwardedThisAct      = relicsAwardedThisAct;
        this.visitedNodeIDs            = visitedNodeIDs;
        this.acquiredRelicIDs          = acquiredRelicIDs;
        this.cardSaveDataCollection    = cardSaveDataCollection;
        this.mapNodeSaveDataCollection = mapNodeSaveDataCollection;
    }
}
