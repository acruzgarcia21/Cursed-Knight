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
    private Canvas        _canvas;
    private RectTransform _canvasRectTransform;

    private Vector2 _originalLocalPointerPosition;
    private Vector2 _pointerDownScreenPosition;
    private Vector2 _selectedPointerOffset;
    
    private Vector3 _originalPanelLocalPosition;
    private Vector3 _originalScale;

    private Player _player;
    
    private bool _cardHasBeenPlayed;
    private bool _playingFromSelection;

    private enum CardState
    {
        Idle,
        Hovering,
        Pressed,
        Dragging,
        Selected,
        Playing
    }

    private CardState _currentState = CardState.Idle;

    private Quaternion _originalRotation;
    private Vector3    _originalPosition;

    private CardDisplay _cardDisplay;
    private HandDisplay _handDisplay;
    
    private CardPlayManager _cardPlayManager;
    private HandManager     _handManager;
    
    private CardVisualEffects _cardVisualEffects;

    private RectTransform _cardPlayPoint;
    private RectTransform _targetingPlayPoint;

    private int _originalSiblingIndex;

    private float _timeMouseClicked;
    
    [SerializeField] private Vector2 cardPlay;

    [FormerlySerializedAs("moveSpeed")]
    [SerializeField] private float lerpFactor         = 10f;
    [SerializeField] private float selectedLerpFactor = 25f;
    [SerializeField] private float dragThreshold      = 15f;
    [SerializeField] private float clickThreshold     = 0.3f;
    
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
        _handManager     = FindFirstObjectByType<HandManager>();
        _handDisplay     = FindFirstObjectByType<HandDisplay>();
        
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
        if ((_currentState == CardState.Selected 
             || (_currentState == CardState.Playing && _playingFromSelection)) && Input.GetMouseButtonDown(1))
        {
            ReturnToIdleState();
            return;
        }
        
        switch (_currentState)
        {
            case CardState.Hovering:
                
                _cardVisualEffects.HandleHoverState(_rectTransform, _originalScale, lerpFactor);
                _cardVisualEffects.HandleRotationToUpright(_rectTransform, lerpFactor);
                _cardVisualEffects.HandleHoverPosition(_rectTransform, _canvasRectTransform, lerpFactor);
                
                break;

            case CardState.Pressed:
                
                _cardVisualEffects.HandleHoverState(_rectTransform, _originalScale, lerpFactor);
                _cardVisualEffects.HandleRotationToUpright(_rectTransform, lerpFactor);
                
                break;

            case CardState.Dragging:
                
                HandleDragState();
                break;

            case CardState.Selected:
                
                _cardVisualEffects.HandleHoverState(_rectTransform, _originalScale, lerpFactor);
                _cardVisualEffects.HandleRotationToUpright(_rectTransform, lerpFactor);
                
                HandleSelectedState();
                
                break;

            case CardState.Playing:
                
                HandlePlayState();
                break;

            case CardState.Idle:
                
                _cardVisualEffects.HandleScaleToNormal(_rectTransform, _originalScale, lerpFactor);
                
                break;
        }
    }

    private void ReturnToIdleState()
    {
        _currentState = CardState.Idle;

        _rectTransform.localRotation = _originalRotation;
        _rectTransform.localPosition = _originalPosition;

        _playingFromSelection = false;

        _handManager.ClearSelectedCard(gameObject);
        _handDisplay.ClearHoveredCard();
        
        _rectTransform.SetSiblingIndex(_originalSiblingIndex);
        
        _handManager.RefreshHandVisuals();
        
        _cardVisualEffects.HandleGlowEffect(false);
        _cardVisualEffects.ShowPlayArrow(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentState != CardState.Idle) return;
        if (!_handDisplay.CanHoverCard(gameObject)) return;

        _originalPosition     = _rectTransform.localPosition;
        _originalRotation     = _rectTransform.localRotation;
        _originalSiblingIndex = _rectTransform.GetSiblingIndex();

        _handDisplay.SetHoveredCard(_originalSiblingIndex, gameObject);
        _handManager.RefreshHandVisuals();
        
        BringCardToFront();
        _currentState = CardState.Hovering;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentState != CardState.Hovering) return;

        ReturnToIdleState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_currentState == CardState.Selected)
        {
            ReturnToIdleState();
            return;
        }

        if (_currentState != CardState.Hovering) return;

        _timeMouseClicked = Time.time;
        _pointerDownScreenPosition = eventData.position;

        _currentState = CardState.Pressed;

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

        if (_currentState == CardState.Playing && !_playingFromSelection)
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

        if (_currentState == CardState.Pressed)
        {
            var holdDuration = Time.time - _timeMouseClicked;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRectTransform,
                    Input.mousePosition,
                    _canvas.worldCamera,
                    out var localPointerPosition))
            {
                ReturnToIdleState();
                return;
            }

            localPointerPosition /= _canvas.scaleFactor;

            _selectedPointerOffset = (Vector2)_rectTransform.localPosition - localPointerPosition;

            if (holdDuration <= clickThreshold)
            {
                _handDisplay.LetActiveCardControlPosition();

                _currentState = CardState.Selected;
                return;
            }

            ReturnToIdleState();
            return;
        }

        if (_currentState == CardState.Dragging)
        {
            ReturnToIdleState();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_currentState != CardState.Pressed && _currentState != CardState.Dragging) return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPointerPosition))
        {
            return;
        }

        if (_currentState == CardState.Pressed)
        {
            var dragDistance = Vector2.Distance(_pointerDownScreenPosition, eventData.position);

            if (dragDistance < dragThreshold) return;

            localPointerPosition /= _canvas.scaleFactor;

            _originalLocalPointerPosition = localPointerPosition;
            _originalPanelLocalPosition = _rectTransform.localPosition;

            _handDisplay.LetActiveCardControlPosition();

            _currentState = CardState.Dragging;
            return;
        }

        localPointerPosition /= _canvas.scaleFactor;

        Vector3 offsetToOriginal =
            localPointerPosition - _originalLocalPointerPosition;

        _rectTransform.localPosition =
            _originalPanelLocalPosition + offsetToOriginal;

        if (_rectTransform.localPosition.y <= cardPlay.y) return;

        _playingFromSelection = false;

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
    
        if (_playingFromSelection && Input.GetMouseButtonDown(0))
        {
            var runtimeCard = _cardDisplay.runtimeCard;
    
            if (UsesTargetingArrow())
            {
                var pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
    
                var targetEnemy = GetEnemyUnderPointer(pointerData);
    
                if (targetEnemy == null) return;
    
                if (_cardPlayManager.TryPlayCard(_player, runtimeCard, gameObject, targetEnemy))
                {
                    _cardHasBeenPlayed = true;
                }
    
                return;
            }
    
            if (_cardPlayManager.TryPlayCard(_player, runtimeCard, gameObject, null))
            {
                _cardHasBeenPlayed = true;
            }
    
            return;
        }
    
        if (Input.mousePosition.y >= cardPlay.y) return;
    
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                Input.mousePosition,
                _canvas.worldCamera,
                out var localPointerPosition))
        {
            localPointerPosition /= _canvas.scaleFactor;
    
            _originalLocalPointerPosition = localPointerPosition;
            _originalPanelLocalPosition = _rectTransform.localPosition;
        }
    
        _currentState = CardState.Dragging;
        _cardVisualEffects.ShowPlayArrow(false);
    }

    private Enemy GetEnemyUnderPointer(PointerEventData eventData)
    {
        if (EventSystem.current == null)
        {
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

        _handDisplay.LetActiveCardControlPosition();
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

    private void BringCardToFront()
    {
        _rectTransform.SetAsLastSibling();
    }

    private void HandleSelectedState()
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                Input.mousePosition,
                _canvas.worldCamera,
                out var localPointerPosition))
        {
            return;
        }

        localPointerPosition /= _canvas.scaleFactor;

        var target = new Vector3(
            localPointerPosition.x + _selectedPointerOffset.x,
            localPointerPosition.y + _selectedPointerOffset.y,
            _rectTransform.localPosition.z);

        _rectTransform.localPosition = Vector3.Lerp(
            _rectTransform.localPosition,
            target,
            selectedLerpFactor * Time.deltaTime);

        if (target.y <= cardPlay.y) return;

        _playingFromSelection = true;
        EnterPlayState();
    }
    
    public void DeselectCard()
    {
        if (_currentState != CardState.Selected && 
            !(_currentState == CardState.Playing && 
              _playingFromSelection))
        {
            return;
        }

        ReturnToIdleState();
    }
}