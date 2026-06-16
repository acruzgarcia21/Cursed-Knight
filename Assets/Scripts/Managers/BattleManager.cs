using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance  { get; private set; }
    
    public EnemyManager EnemyManager { get; private set; }
    public TurnManager TurnManager { get; private set; }
    public HandManager HandManager { get; private set; }
    public DrawPileManager DrawPileManager { get; private set; }
    public DiscardManager DiscardManager { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeMangers();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMangers()
    {
        EnemyManager    = GetComponentInChildren<EnemyManager>();
        TurnManager     = GetComponentInChildren<TurnManager>();
        HandManager     = GetComponentInChildren<HandManager>();
        DrawPileManager = GetComponentInChildren<DrawPileManager>();
        DiscardManager  = GetComponentInChildren<DiscardManager>();

        if (EnemyManager == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/EnemyManager");
            if (prefab == null)
            {
                Debug.Log($"Enemy prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                EnemyManager = GetComponentInChildren<EnemyManager>();
            }
        }

        if (TurnManager == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/TurnManager");
            if (prefab == null)
            {
                Debug.Log($"TurnManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                TurnManager = GetComponentInChildren<TurnManager>();
            }
        }

        if (HandManager == null) 
        {
            var prefab = Resources.Load<GameObject>("Prefabs/HandManager");
            if (prefab == null)
            {
                Debug.Log($"HandManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                HandManager = GetComponentInChildren<HandManager>();
            }
        }

        if (DrawPileManager == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/DrawPileManager");
            if (prefab == null)
            {
                Debug.Log($"DrawPileManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                DrawPileManager = GetComponentInChildren<DrawPileManager>();
            }
        }

        if (DiscardManager == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/DiscardManager");
            if (prefab == null)
            {
                Debug.Log($"DiscardManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                DiscardManager = GetComponentInChildren<DiscardManager>();
            }
        }
    }
}
