using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public OptionsManager OptionsManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    private DeckManager DeckManager { get; set; }
    private UIManager UIManager { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMangers();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMangers()
    {
        OptionsManager = GetComponentInChildren<OptionsManager>();
        AudioManager   = GetComponentInChildren<AudioManager>();
        DeckManager    = GetComponentInChildren<DeckManager>();

        if (OptionsManager == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/OptionsManager");
            if (prefab == null)
            {
                Debug.Log($"OptionsManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                OptionsManager = GetComponentInChildren<OptionsManager>();
            }
        }

        if (AudioManager == null)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/AudioManager");
            if (prefab == null)
            {
                Debug.Log($"AudioManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                AudioManager = GetComponentInChildren<AudioManager>();
            }
        }

        if (DeckManager == null) 
        {
            var prefab = Resources.Load<GameObject>("Prefabs/DeckManager");
            if (prefab == null)
            {
                Debug.Log($"DeckManager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                DeckManager = GetComponentInChildren<DeckManager>();
            }
        }
        
        if (UIManager == null) 
        {
            var prefab = Resources.Load<GameObject>("Prefabs/UIManager");
            if (prefab == null)
            {
                Debug.Log($"UI Manager prefab not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                UIManager = GetComponentInChildren<UIManager>();
            }
        }
    }
}
