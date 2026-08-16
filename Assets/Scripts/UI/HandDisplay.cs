using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public float fanSpread       = 7.5f;
    public float cardSpacing     = 100f;
    public float verticalSpacing = 100f;

    private int _hoveredCardIndex = -1;

    [SerializeField] private float hoveredCardExtraWidth = 250f;

    public void UpdateHandVisuals(List<GameObject> cardsInHand)
    {
        var cardCount = cardsInHand.Count;

        if (cardCount == 0) return;

        // Error handling for 1 card in hand
        if (cardCount == 1)
        {
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }

        var totalHandWidth = (cardCount - 1) * cardSpacing;

        if (_hoveredCardIndex >= 0)
        {
            totalHandWidth += hoveredCardExtraWidth;
        }

        var currentX = -totalHandWidth / 2f;

        for (var i = 0; i < cardCount; i++)
        {
            // Goes through every single card and goes through each rotation
            var rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            // Give the hovered card extra room before it
            if (i == _hoveredCardIndex)
            {
                if (_hoveredCardIndex == cardCount - 1)
                {
                    currentX += hoveredCardExtraWidth;
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
            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);

            // Move to the next normal card slot
            currentX += cardSpacing;

            // Give the hovered card extra room after it
            if (i == _hoveredCardIndex)
            {
                if (_hoveredCardIndex == 0)
                {
                    currentX += hoveredCardExtraWidth;
                }
                else if (_hoveredCardIndex < cardCount - 1)
                {
                    currentX += hoveredCardExtraWidth / 2f;
                }
            }
        }
    }

    public void SetHoveredCard(int cardIndex)
    {
        _hoveredCardIndex = cardIndex;
    }

    public void ClearHoveredCard()
    {
        _hoveredCardIndex = -1;
    }
}