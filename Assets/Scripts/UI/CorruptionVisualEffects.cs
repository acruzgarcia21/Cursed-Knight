using System;
using UnityEngine;

public class CorruptionVisualEffects : MonoBehaviour
{
    [SerializeField] private RectTransform corruptionUI;
    
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseSpeed = 8f;
    [SerializeField] private float scaleIncreaseThreshold = 0.01f;

    private Vector3 _originalScale;
    
    private bool _isPulsing;
    private bool _isGrowing;

    private void Awake()
    {
        _originalScale = corruptionUI.localScale;
        _isPulsing = false;
    }

    private void Update()
    {
        if (_isPulsing == false) return;

        if (_isGrowing)
        {
            var targetScale = _originalScale * pulseScale;
            
            corruptionUI.localScale = Vector3.Lerp(
                corruptionUI.localScale, 
                targetScale, 
                pulseSpeed * Time.deltaTime
                );

            if (Vector3.Distance(corruptionUI.localScale, targetScale) < scaleIncreaseThreshold)
            {
                _isGrowing = false;
            }
        }
        else if (!_isGrowing)
        {
            corruptionUI.localScale = Vector3.Lerp(
                corruptionUI.localScale, 
                _originalScale, 
                pulseSpeed * Time.deltaTime
            );
            
            if (Vector3.Distance(corruptionUI.localScale, _originalScale) < scaleIncreaseThreshold)
            {
                corruptionUI.localScale = _originalScale;
                _isPulsing = false;
            }
        }
    }

    public void PlayActivationPulse()
    {
        _isPulsing = true;
        _isGrowing = true;
        
        
    }
}
