using UnityEngine;

public class EnemyVisualEffects : MonoBehaviour
{
    private Vector3 _originalPosition;

    private float _shakeTimer;

    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeStrength = 10f;

    [SerializeField] private RectTransform enemySpritePosition; 

    private void Awake()
    {
        _originalPosition = enemySpritePosition.transform.localPosition;
    }

    private void Update()
    {
        if (_shakeTimer > 0)
        {
            var randomX = Random.Range(-shakeStrength, shakeStrength);
            var randomY = Random.Range(-shakeStrength, shakeStrength);

            var offset = new Vector3(randomX, randomY, 0);

            enemySpritePosition.localPosition = _originalPosition + offset;

            _shakeTimer -= Time.deltaTime;
            return;
        }

        enemySpritePosition.localPosition = _originalPosition;
    }

    public void ApplyShake()
    {
        _shakeTimer = shakeDuration;
    }
}
