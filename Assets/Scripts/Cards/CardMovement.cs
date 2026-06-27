using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using CursedKnight;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    // For object
    private RectTransform _rectTransform;
    private Canvas _canvas; // Looking for parent canvas
    
    // Cached to save calls
    private RectTransform _canvasRectTransform;
    
    // Stores mouse pointer position
    private Vector2 _originalLocalPointerPosition;
    
    // Store original card location and scale
    private Vector3 _originalPanelLocalPosition;
    private Vector3 _originalScale;

    private Player _player;
    
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
    
    private Card _cardData;
    private CardDisplay _cardDisplay;
    private CardPlayManager _cardPlayManager;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow; // Enables dynamic arrow where we want to play our card
    // Controls how aggressive the play effect moves to target position
    [FormerlySerializedAs("moveSpeed")] [SerializeField] private float lerpFactor = 10f;

    private void Awake()
    {
        _cardDisplay    = GetComponent<CardDisplay>();
        _rectTransform  = GetComponent<RectTransform>();
        
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null) return;
        
        _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        
        _originalScale    = _rectTransform.localScale;
        _originalPosition = _rectTransform.localPosition;
        _originalRotation = _rectTransform.localRotation;
        
        _cardData = _cardDisplay.cardData;
        
        _player          = FindFirstObjectByType<Player>();
        _cardPlayManager = FindFirstObjectByType<CardPlayManager>();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case CardState.Hovering:
                HandleHoverState();
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
        _currentState                = CardState.Idle;
        _rectTransform.localScale    = _originalScale;       // Reset Scale
        _rectTransform.localRotation = _originalRotation;    // Reset Rotation
        _rectTransform.localPosition = _originalPosition;    // Reset Position
        glowEffect.SetActive(false);                         // Disable Glow Effect
        playArrow.SetActive(false);                          // Disable playArrow
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentState != CardState.Idle) return;
        _originalPosition = _rectTransform.localPosition;
        _originalRotation = _rectTransform.localRotation;
        _originalScale = _rectTransform.localScale;
            
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
        _currentState = CardState.Dragging; // Drag State
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,                      // Gets position of mouse on screen
            eventData.position,              // Current mouse position in screen coordinates
            eventData.pressEventCamera,                // The camera that registered the click
            out _originalLocalPointerPosition);        // Stores the calculated mouse position relative to the canvas
        
        // Stores the position where the click happened
        _originalPanelLocalPosition = _rectTransform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _cardData = _cardDisplay.cardData;
        
        if (_rectTransform.localPosition.y > cardPlay.y)
        {
            if (!_cardPlayManager.TryPlayCard(_player, _cardData, gameObject))
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
                out var localPointerPosition)) return;
        
        // Ensures the card follows the mouse and not just the mouse movement
        localPointerPosition /= _canvas.scaleFactor;
        
        Vector3 offsetToOriginal =  localPointerPosition - _originalLocalPointerPosition;
        _rectTransform.localPosition = _originalPanelLocalPosition + offsetToOriginal;

        if (!(_rectTransform.localPosition.y > cardPlay.y)) return;
        
        _currentState = CardState.Playing;
        playArrow.SetActive(true);
    }

    private void HandleHoverState()
    {
        glowEffect.SetActive(true);
        _rectTransform.localScale = _originalScale * selectScale;
    }

    private void HandleDragState()
    {
        // Set the card's rotation to zero
        _rectTransform.localRotation = Quaternion.identity;
    }

    private void HandlePlayState()
    {
        _rectTransform.localPosition = Vector3.Lerp(
            _rectTransform.localPosition, 
            playPosition, 
            lerpFactor * Time.deltaTime); // Moves based on elapsed time
        
        _rectTransform.localRotation = Quaternion.identity;
        
        if (Vector3.Distance(_rectTransform.localPosition, playPosition) < 0.1f)
        {
            _rectTransform.localPosition = playPosition;
        }

        if (!(Input.mousePosition.y < cardPlay.y)) return;
        
        _currentState = CardState.Dragging;
        playArrow.SetActive(false);
    }
}
