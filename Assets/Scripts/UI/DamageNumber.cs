using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    
    public void UpdateDamageText(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
    }
}
