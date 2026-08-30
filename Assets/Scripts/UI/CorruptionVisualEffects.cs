using System;
using UnityEngine;

public class CorruptionVisualEffects : MonoBehaviour
{
    [SerializeField] private RectTransform corruptionUI;
    
    // Initial Feedback For Corruption Overload
    [Header("Initial Feedback")] 
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseSpeed = 8f;
    [SerializeField] private float scaleThreshold = 0.001f;

    private Vector3 _originalScale;
    
    private bool _isPulsing;
    private bool _isGrowing;
    
    // Persistent Feedback When Corrupted
    [Space(10)] [Header("Persistent Feedback")]
    [SerializeField] private float persistentPulseScale = 1.05f;
    [SerializeField] private float persistentPulseSpeed = 2f;

    private bool _isPlayerCorrupted;
    private bool _isPersistentGrowing;
    
    // Reset Feedback When No Longer Corrupted
    private bool _isReturningToNormal;
    
    private void Awake()
    {
        _originalScale = corruptionUI.localScale;
        _isPulsing = false;
    }

    private void Update()
    {
        if (_isPulsing)
        {
            if (_isGrowing)
            {
                var targetScale = _originalScale * pulseScale;

                corruptionUI.localScale = Vector3.Lerp(
                    corruptionUI.localScale,
                    targetScale,
                    pulseSpeed * Time.deltaTime
                );

                if (Vector3.Distance(corruptionUI.localScale, targetScale) < scaleThreshold)
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

                if (Vector3.Distance(corruptionUI.localScale, _originalScale) < scaleThreshold)
                {
                    corruptionUI.localScale = _originalScale;
                    _isPulsing = false;
                }
            }
        }

        if (_isPlayerCorrupted && !_isPulsing)
        {
            if (_isPersistentGrowing)
            {
                var targetScale = _originalScale * persistentPulseScale;

                corruptionUI.localScale = Vector3.Lerp(
                    corruptionUI.localScale,
                    targetScale,
                    persistentPulseSpeed * Time.deltaTime
                );
                
                if (Vector3.Distance(corruptionUI.localScale, targetScale) < scaleThreshold)
                {
                    _isPersistentGrowing = false;
                }
            }
            else if (!_isPersistentGrowing)
            {
                corruptionUI.localScale = Vector3.Lerp(
                    corruptionUI.localScale,
                    _originalScale,
                    persistentPulseSpeed * Time.deltaTime
                );

                if (Vector3.Distance(corruptionUI.localScale, _originalScale) < scaleThreshold)
                {
                    corruptionUI.localScale = _originalScale;
                    _isPersistentGrowing = true;
                }
            }
        }

        if (_isReturningToNormal && !_isPlayerCorrupted && !_isPulsing)
        {
            corruptionUI.localScale = Vector3.Lerp(
                corruptionUI.localScale,
                _originalScale,
                persistentPulseSpeed * Time.deltaTime
            );

            if (Vector3.Distance(corruptionUI.localScale, _originalScale) < scaleThreshold)
            {
                corruptionUI.localScale = _originalScale;
                _isReturningToNormal = false;
            }
        }
    }

    public void PlayActivationPulse()
    {
        _isPulsing = true;
        _isGrowing = true;
    }

    public void SetPlayerIsCorrupted()
    {
        _isPlayerCorrupted = true;
    }

    public void SetPlayerIsNotCorrupted()
    {
        _isPlayerCorrupted = false;
        _isReturningToNormal = true;
    }
}
