using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ActEnvironmentDisplay : MonoBehaviour
{
    [Header("Battle Backgrounds")]
    [SerializeField] private Image battleBackground;

    [SerializeField] private Sprite actOneBackground;
    [SerializeField] private Sprite actTwoBackground;
    [SerializeField] private Sprite actThreeBackground;
    [SerializeField] private Sprite actThreeFinalBossBackground;
    
    // [Space(10)] [Header("Act Atmospheres")]
    [SerializeField] private GameObject actOneAtmosphere;
    [SerializeField] private GameObject actTwoAtmosphere;

    public void UpdateEnvironment(RunManager.CurrentAct currentAct, Map.MapNodeType mapNodeType)
    {
        var sprite = currentAct switch
        {
            RunManager.CurrentAct.ActOne => actOneBackground,
            RunManager.CurrentAct.ActTwo => actTwoBackground,
            RunManager.CurrentAct.ActThree => mapNodeType == Map.MapNodeType.Boss
                ? actThreeFinalBossBackground
                : actThreeBackground,
            _ => null
        };
        
        if (sprite == null)
        {
            Debug.LogError("ActEnvironmentDisplay: Sprite is null!");
            return;
        }

        battleBackground.sprite = sprite;

        if (actOneAtmosphere != null)
            actOneAtmosphere.SetActive(currentAct == RunManager.CurrentAct.ActOne);

        if (actTwoAtmosphere != null)
            actTwoAtmosphere.SetActive(currentAct == RunManager.CurrentAct.ActTwo);
        
    }
}
