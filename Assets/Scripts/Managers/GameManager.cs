using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public OptionsManager OptionsManager { get; private set; }
    public AudioManager AudioManager { get; private set; }

    public enum RunStartRequest
    {
        None,
        NewRun,
        Continue
    }

    private RunStartRequest _runStartRequest = RunStartRequest.None;

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
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void SetRunStartRequest(RunStartRequest runStartRequest)
    {
        _runStartRequest = runStartRequest;
    }

    public RunStartRequest ConsumeRunStartRequest()
    {
        var currentRequest = _runStartRequest;
        _runStartRequest = RunStartRequest.None;

        return currentRequest;
    }

    private void InitializeMangers()
    {
        OptionsManager = GetComponentInChildren<OptionsManager>();
        AudioManager   = GetComponentInChildren<AudioManager>();

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
    }
}
