using UnityEngine;

public class CombatFeedbackManager : MonoBehaviour
{
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Transform combatFeedbackPoint;

    public void ShowDamageNumber(int damageAmount)
    {
        if (damageAmount <= 0) return;
        
        var damageNum = 
            Instantiate(damageNumberPrefab, combatFeedbackPoint);
        
        damageNum.transform.localPosition = Vector3.zero;
        damageNum.transform.localRotation = Quaternion.identity;
        damageNum.transform.localScale = Vector3.one;

        var damageNumUI = damageNum.GetComponent<DamageNumber>();
        
        damageNumUI.UpdateDamageText(damageAmount);
    }
}
