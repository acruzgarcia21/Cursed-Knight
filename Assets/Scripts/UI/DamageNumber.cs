using System;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float floatDistance = 100f;
    [SerializeField] private float lifeTime = 1f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private float _elapsedTime;
    private float _progress;
    
    public void Update()
    {
        UpdateAnimationProgress();
        MoveDamageNumber();
        FadeDamageNumber();
        CheckLifetime();
        
    }

    public void Start()
    {
        _startPosition = transform.localPosition;
        _targetPosition = _startPosition + Vector3.up * floatDistance;
    }

    public void UpdateDamageText(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
    }

    private void MoveDamageNumber()
    {
        transform.localPosition = Vector3.Lerp(
            _startPosition,
            _targetPosition,
            _progress
        );
    }

    private void CheckLifetime()
    {
        if (_elapsedTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void FadeDamageNumber()
    {
        var alpha = 1 - _progress;
        var currentColor = damageText.color;
        currentColor.a = alpha;
        damageText.color = currentColor;
    }

    private void UpdateAnimationProgress()
    {
        _elapsedTime += Time.deltaTime;
        _progress = _elapsedTime / lifeTime;
    }
}