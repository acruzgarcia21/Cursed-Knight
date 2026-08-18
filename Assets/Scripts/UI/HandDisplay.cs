using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public float fanSpread       = 7.5f;
    public float cardSpacing     = 100f;
    public float verticalSpacing = 100f;
    
    private int _hoveredCardIndex = -1;
    
    private GameObject _hoveredCard;

    private bool _activeCardControlsOwnPosition;

    private readonly Dictionary<GameObject, Vector3>    _cardTargetPositions = new();
    private readonly Dictionary<GameObject, Quaternion> _cardTargetRotations = new();

    [SerializeField] private float handLerpFactor = 10f;
    [SerializeField] private float hoveredCardExtraWidth = 250f;
    [SerializeField] private float edgeHoveredExtraWidth = 250f;

    public void Update()
    {
        foreach (var (card, targetPosition) in _cardTargetPositions)
        {
            if (card == null) continue;

            if (card == _hoveredCard)
            {
                if (_activeCardControlsOwnPosition) continue;
                
                var currentPosition = card.transform.localPosition;
                var hoveredTargetPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);

                card.transform.localPosition = Vector3.Lerp(
                    currentPosition,
                    hoveredTargetPosition,
                    handLerpFactor * Time.deltaTime);

                continue;
            }

            card.transform.localPosition = Vector3.Lerp(
                card.transform.localPosition,
                targetPosition,
                handLerpFactor * Time.deltaTime);
        }

        foreach (var (card, targetRotation) in _cardTargetRotations)
        {
            if (card == null) continue;
            if (card == _hoveredCard) continue;
            
            card.transform.localRotation = Quaternion.Lerp(
                card.transform.localRotation,
                targetRotation,
                handLerpFactor * Time.deltaTime);
        }
    }

    public void UpdateHandVisuals(List<GameObject> cardsInHand)
    {
        var cardCount = cardsInHand.Count;

        if (cardCount == 0) return;

        // Error handling for 1 card in hand
        if (cardCount == 1)
        {
            _cardTargetRotations[cardsInHand[0]] = Quaternion.Euler(0f, 0f, 0f);

            var targetPosition = new Vector3(0f, 0f, 0f);
            _cardTargetPositions[cardsInHand[0]] = targetPosition;

            return;
        }

        var totalHandWidth = (cardCount - 1) * cardSpacing;

        if (_hoveredCardIndex >= 0)
        {
            if (_hoveredCardIndex == 0 || _hoveredCardIndex == cardCount - 1)
            {
                totalHandWidth += edgeHoveredExtraWidth;
            }
            else
            {
                totalHandWidth += hoveredCardExtraWidth;
            }
        }

        var currentX = -totalHandWidth / 2f;

        for (var i = 0; i < cardCount; i++)
        {
            // Goes through every single card and goes through each rotation
            var rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
            
            _cardTargetRotations[cardsInHand[i]] = Quaternion.Euler(0f, 0f, rotationAngle);

            // Give the hovered card extra room before it
            if (i == _hoveredCardIndex)
            {
                if (_hoveredCardIndex == cardCount - 1)
                {
                    currentX += edgeHoveredExtraWidth;
                }
                else if (_hoveredCardIndex > 0)
                {
                    currentX += hoveredCardExtraWidth / 2f;
                }
            }

            var horizontalOffset = currentX;

            // Normalize card position between -1 and 1
            var normalizedPosition = (2f * i / (cardCount - 1f) - 1f);
            var verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

            // Set card positions
            var targetPosition = new Vector3(horizontalOffset, verticalOffset, 0f);

            _cardTargetPositions[cardsInHand[i]] = targetPosition;

            // Move to the next normal card slot
            currentX += cardSpacing;

            // Give the hovered card extra room after it
            if (i == _hoveredCardIndex)
            {
                if (_hoveredCardIndex == 0)
                {
                    currentX += edgeHoveredExtraWidth;
                }
                else if (_hoveredCardIndex < cardCount - 1)
                {
                    currentX += hoveredCardExtraWidth / 2f;
                }
            }
        }
    }

    public void SetHoveredCard(int cardIndex, GameObject hoveredCard)
    {
        _hoveredCardIndex = cardIndex;
        _hoveredCard = hoveredCard;
    }

    public void ClearHoveredCard()
    {
        _hoveredCardIndex = -1;
        _hoveredCard = null;
        
        LetHandControlPosition();
    }

    public void RemoveCard(GameObject card)
    {
        _cardTargetPositions.Remove(card);
        _cardTargetRotations.Remove(card);

        if (card == _hoveredCard)
        {
            _hoveredCardIndex = -1;
            _hoveredCard = null;
            
            LetHandControlPosition();
        }
        
    }

    public void LetActiveCardControlPosition()
    {
        _activeCardControlsOwnPosition = true;      
    }

    public void LetHandControlPosition()
    {
        _activeCardControlsOwnPosition = false;
    }
    
    public bool CanHoverCard(GameObject card)
    {
        return _hoveredCard == null || _hoveredCard == card;
    }
}