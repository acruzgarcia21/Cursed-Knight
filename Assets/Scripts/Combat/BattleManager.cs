using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance  { get; private set; }
     public EnemyManager EnemyManager { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        EnemyManager = GetComponentInChildren<EnemyManager>();

        if (EnemyManager == null)
        {
            Debug.Log("EnemyManager not found under BattleManager");
        }
    }

    private void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        EnemyManager.BattleSetup();
    }

    public void WinBattle()
    {
        Debug.Log("Battle won");
    }

    public void LoseBattle()
    {
        Debug.Log("Battle lost");
    }
}
