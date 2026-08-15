using System.Collections.Generic;
using CursedKnight;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CardMovement : MonoBehaviour,
    IDragHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private RectTransform _canvasRectTransform;

    private Vector2 _originalLocalPointerPosition;
    private Vector3 _originalPanelLocalPosition;
    private Vector3 _originalScale;

    private Player _player;
    private bool _cardHasBeenPlayed;

    private enum CardState
    {
        Idle,
        Hovering,
        Dragging,
        Playing
    }

    private CardState _currentState = CardState.Idle;

    private Quaternion _originalRotation;
    private Vector3 _originalPosition;

    private CardDisplay _cardDisplay;
    private CardPlayManager _cardPlayManager;
    private CardVisualEffects _cardVisualEffects;

    private RectTransform _cardPlayPoint;
    private RectTransform _targetingPlayPoint;
    
    [SerializeField] private Vector2 cardPlay;

    [FormerlySerializedAs("moveSpeed")]
    [SerializeField] private float lerpFactor = 10f;

    [SerializeField] private float targetingScale = 1.5f;

    private void Awake()
    {
        _cardDisplay       = GetComponent<CardDisplay>();
        _rectTransform     = GetComponent<RectTransform>();
        _cardVisualEffects = GetComponent<CardVisualEffects>();

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null) return;

        _canvasRectTransform = _canvas.GetComponent<RectTransform>();

        _originalScale    = _rectTransform.localScale;
        _originalPosition = _rectTransform.localPosition;
        _originalRotation = _rectTransform.localRotation;

        _player          = FindFirstObjectByType<Player>();
        _cardPlayManager = FindFirstObjectByType<CardPlayManager>();
        
        var playPoint = FindFirstObjectByType<CardPlayPoint>();

        if (playPoint != null)
        {
            _cardPlayPoint = playPoint.GetComponent<RectTransform>();
        }

        var targetPlayPoint = FindFirstObjectByType<TargetingCardPoint>();
        if (targetPlayPoint != null)
        {
            _targetingPlayPoint = targetPlayPoint.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        switch (_currentState)
        {
            case CardState.Hovering:
                _cardVisualEffects.HandleHoverState(
                    _rectTransform,
                    _originalScale,
                    lerpFactor
                );
                break;

            case CardState.Dragging:
                HandleDragState();
                break;

            case CardState.Playing:
                HandlePlayState();
                break;

            case CardState.Idle:
            default:
                break;
        }
    }

    private void ReturnToIdleState()
    {
        _currentState = CardState.Idle;

        _rectTransform.localScale    = _originalScale;
        _rectTransform.localRotation = _originalRotation;
        _rectTransform.localPosition = _originalPosition;

        _cardVisualEffects.HandleGlowEffect(false);
        _cardVisualEffects.ShowPlayArrow(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentState != CardState.Idle) return;

        _originalPosition = _rectTransform.localPosition;
        _originalRotation = _rectTransform.localRotation;
        _originalScale    = _rectTransform.localScale;

        _currentState = CardState.Hovering;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentState != CardState.Hovering) return;

        ReturnToIdleState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_currentState != CardState.Hovering) return;

        _currentState = CardState.Dragging;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _originalLocalPointerPosition
        );

        _originalPanelLocalPosition = _rectTransform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_cardHasBeenPlayed) return;

        var runtimeCard = _cardDisplay.runtimeCard;

        if (runtimeCard == null || runtimeCard.cardData == null)
        {
            Debug.LogWarning("Card has no RuntimeCard data.");
            ReturnToIdleState();
            return;
        }

        var targetEnemy = GetEnemyUnderPointer(eventData);

        if (_currentState == CardState.Playing)
        {
            var cardWasPlayed = _cardPlayManager.TryPlayCard(
                _player,
                runtimeCard,
                gameObject,
                targetEnemy
            );

            if (cardWasPlayed)
            {
                _cardHasBeenPlayed = true;
            }
            else
            {
                ReturnToIdleState();
            }

            return;
        }

        ReturnToIdleState();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_currentState != CardState.Dragging) return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPointerPosition))
        {
            return;
        }

        localPointerPosition /= _canvas.scaleFactor;

        Vector3 offsetToOriginal =
            localPointerPosition - _originalLocalPointerPosition;

        _rectTransform.localPosition =
            _originalPanelLocalPosition + offsetToOriginal;

        if (_rectTransform.localPosition.y <= cardPlay.y) return;

        EnterPlayState();
    }

    private void HandleDragState()
    {
        _rectTransform.localRotation = Quaternion.identity;
    }

    private void HandlePlayState()
    {
        if (UsesTargetingArrow())
        {
            HandleTargetingPlayState();
        }
        else
        {
            HandleCenterPlayState();
        }

        if (Input.mousePosition.y >= cardPlay.y) return;

        _currentState = CardState.Dragging;
        _cardVisualEffects.ShowPlayArrow(false);
    }

    private Enemy GetEnemyUnderPointer(PointerEventData eventData)
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("No EventSystem found.");
            return null;
        }

        var raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var result in raycastResults)
        {
            var enemy = result.gameObject.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                return enemy;
            }
        }

        return null;
    }

    private bool UsesTargetingArrow()
    {
        var currentCard = _cardDisplay.runtimeCard;

        if (currentCard == null) return false;
        
        return currentCard.cardData.targetType == Card.TargetType.SingleEnemy;
    }

    private void EnterPlayState()
    {
        _currentState = CardState.Playing;

        _cardVisualEffects.ShowPlayArrow(UsesTargetingArrow());
    }

    private void HandleTargetingPlayState()
    {
        var targetPosition = _rectTransform.parent.InverseTransformPoint(_targetingPlayPoint.position);
        
        _rectTransform.localPosition = Vector3.Lerp(
            _rectTransform.localPosition,
            targetPosition,
            lerpFactor * Time.deltaTime
        );

        _rectTransform.localRotation = Quaternion.identity;

        if (Vector3.Distance(_rectTransform.localPosition, targetPosition) < 0.1f)
        {
            _rectTransform.localPosition = targetPosition;
        }
    }

    private void HandleCenterPlayState()
    {
        var targetPosition = _rectTransform.parent.InverseTransformPoint(_cardPlayPoint.position);
        
        _rectTransform.localPosition = Vector3.Lerp(
            _rectTransform.localPosition,
            targetPosition,
            lerpFactor * Time.deltaTime
        );

        _rectTransform.localRotation = Quaternion.identity;

        if (Vector3.Distance(_rectTransform.localPosition, targetPosition) < 0.1f)
        {
            _rectTransform.localPosition = targetPosition;
        }
    }
}