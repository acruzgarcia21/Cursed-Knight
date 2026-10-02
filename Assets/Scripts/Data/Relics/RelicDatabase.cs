using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Relic Database", menuName = "RelicDatabase")]
public class RelicDatabase : ScriptableObject
{
    [SerializeField] private List<RelicData> persistentRelicDefinitions;

    private readonly Dictionary<string, RelicData> _relicDictionary = new();
    
    private void OnEnable()
    {
        _relicDictionary.Clear();

        foreach (var relic in persistentRelicDefinitions)
        {
            if (relic == null || string.IsNullOrEmpty(relic.GetRelicID()))
            {
                Debug.LogError("Relic is not Valid!");
                continue;
            }

            if (_relicDictionary.ContainsKey(relic.GetRelicID()))
            {
                Debug.LogError("Dictionary already contains relic!");
                continue;
            }
            
            _relicDictionary.Add(relic.GetRelicID(), relic);
        }
    }

    public RelicData GetRelicByID(string relicID)
    {
        if (_relicDictionary.TryGetValue(relicID, out var relic))
        {
            return relic;
        }
        
        Debug.LogError("Dictionary does not contain the relic associated with the provided key!");
        return null;
    }
}
