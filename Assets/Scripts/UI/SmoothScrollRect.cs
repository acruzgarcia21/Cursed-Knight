using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmoothScrollRect : MonoBehaviour, IScrollHandler
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private float scrollStep = 0.12f;
    [SerializeField] private float smoothSpeed = 12f;

    private float _targetVerticalPosition = 1f;

    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        _targetVerticalPosition = scrollRect.verticalNormalizedPosition;
    }

    private void Update()
    {
        scrollRect.verticalNormalizedPosition = Mathf.Lerp(
            scrollRect.verticalNormalizedPosition,
            _targetVerticalPosition,
            smoothSpeed * Time.deltaTime);
        
        if (Mathf.Abs(scrollRect.verticalNormalizedPosition - _targetVerticalPosition) < 0.01f)
            scrollRect.verticalNormalizedPosition = _targetVerticalPosition;
    }

    public void OnScroll(PointerEventData eventData)
    {
        _targetVerticalPosition += eventData.scrollDelta.y * scrollStep;
        _targetVerticalPosition = Mathf.Clamp01(_targetVerticalPosition);
    }
}