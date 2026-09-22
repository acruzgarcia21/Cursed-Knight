using System;
using UnityEngine;

public class RestManager : MonoBehaviour
{
    private Player _player;
    
    private const float RestHealScale = 0.30f;

    private void Awake()
    {
        _player = FindAnyObjectByType<Player>();
    }

    public void OnHealSelection()
    {
        if (_player == null) return;

        var playerMaxHealth = _player.GetMaxHealth();
        var healthToGain = Mathf.CeilToInt(playerMaxHealth * RestHealScale);
        
        _player.Heal(healthToGain);
    }
}
