using System.Collections;
using UnityEngine;

public class EnemyVisualEffects : MonoBehaviour
{
    private OptionsManager _optionsManager;
    private Vector3 _originalPosition;
    private Vector3 _originalObjectPosition;

    private float _shakeTimer;
    private float _moveTimer;
    
    private bool _isReturning;
    
    [Header("General")]
    [SerializeField] private RectTransform enemySpritePosition;

    [SerializeField] private RectTransform enemyObjectPosition;

    [Space(10)] [Header("Take Damage Animation")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeStrength = 10f;

    [Space(10)] [Header("Deal Damage Animation")]
    [SerializeField] private float moveDuration = 0.15f;

    [SerializeField] private float moveDistance = 50f;

    [Space(10)] [Header("Enemy Death Animation")] 
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float fadeDuration = 0.15f;

    private Vector3 _targetPosition;

    private float _elapsedTime;
    private float _fadeElapsedTime;
    private float _fadeProgress;
    private float _progress;


    private void Awake()
    {
        _optionsManager = FindAnyObjectByType<OptionsManager>();
        _originalPosition = enemySpritePosition.transform.localPosition;
        _targetPosition = _originalPosition + Vector3.left * moveDistance;
    }

    private void Update()
    {
        if (_optionsManager.GetReduceMotion())
        {
            _shakeTimer = 0f;
            _moveTimer = 0f;
            _isReturning = true;
            enemySpritePosition.localPosition = _originalPosition;
            return;
        }

        if (_shakeTimer > 0)
        {
            var randomX = Random.Range(-shakeStrength, shakeStrength);
            var randomY = Random.Range(-shakeStrength, shakeStrength);

            var offset = new Vector3(randomX, randomY, 0);

            enemySpritePosition.localPosition = _originalPosition + offset;

            _shakeTimer -= Time.deltaTime;
            return;
        }

        if (_moveTimer > 0)
        {
            _moveTimer -= Time.deltaTime;
            UpdateAnimationProgress();

            if (_isReturning)
            {
                MoveEnemySpriteBackToOriginalPosition(_targetPosition);
            }
            else
            {
                MoveEnemySprite();
            }

            if (!(_progress >= 1f)) return;
            if (_isReturning) return;
            
            _isReturning = true;
            _elapsedTime = 0f;
            _progress = 0f;
            _moveTimer = moveDuration;

            return;
        }

        enemySpritePosition.localPosition = _originalPosition;
    }
    
    public bool HasReachedImpact()
    {
        return _isReturning;
    }

    public bool IsMoving()
    {
        return _moveTimer > 0;
    }

    public void ApplyShake()
    {
        if (_optionsManager.GetReduceMotion()) return;
        _shakeTimer = shakeDuration;
    }

    public void ApplyActionMove()
    {
        ApplyMove();
        _targetPosition = _originalPosition + Vector3.up * (moveDistance * 0.25f);
    }

    public void ApplyMove()
    {
        _targetPosition = _originalPosition + Vector3.left * moveDistance;
        _elapsedTime = 0f;
        _progress = 0f;
        _isReturning = false;
        _moveTimer = moveDuration;
    }

    public IEnumerator FadeEnemy()
    {
        _shakeTimer      = 0f;
        _moveTimer       = 0f;
        _fadeElapsedTime = 0f;

        while (_fadeElapsedTime < fadeDuration)
        {
            UpdateDeathAnimationProgress();

            canvasGroup.alpha = 1 - _fadeProgress;
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

    private void MoveEnemySprite()
    {
        enemySpritePosition.transform.localPosition = Vector3.Lerp(
            _originalPosition,
            _targetPosition,
            _progress
        );
    }

    private void MoveEnemySpriteBackToOriginalPosition(Vector3 startingPosition)
    {
        enemySpritePosition.transform.localPosition = Vector3.Lerp(
            startingPosition,
            _originalPosition,
            _progress
        );
    }

    private void UpdateAnimationProgress()
    {
        _elapsedTime += Time.deltaTime;
        _progress = moveDuration > 0f ? Mathf.Clamp01(_elapsedTime / moveDuration) : 1f;
    }

    private void UpdateDeathAnimationProgress()
    {
        _fadeElapsedTime += Time.deltaTime;
        _fadeProgress = fadeDuration > 0f ? Mathf.Clamp01(_fadeElapsedTime / fadeDuration) : 1f;
    }
}
