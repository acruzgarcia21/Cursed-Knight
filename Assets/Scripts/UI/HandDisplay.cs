using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public float fanSpread       = 7.5f;
    public float cardSpacing     = 100f;
    public float verticalSpacing = 100f;
    
    public void UpdateHandVisuals(List<GameObject> cardsInHand)
    {
        var cardCount = cardsInHand.Count;

        // Error handling for 1 card in hand
        if (cardCount == 1)
        {
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }
        
        for (var i = 0; i < cardCount; i++)
        {
            // Goes through every single card and goes through each rotation
            var rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            // Helps cards visually offset in both vertical and horizontal so that cards are not stacked on each other
            var horizontalOffset = (cardSpacing * (i - (cardCount - 1) / 2f));
            // Normalize card position between -1 and 1
            var normalizedPosition = (2f * i / (cardCount - 1f) - 1f);
            var verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);
            
            // Set card positions
            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
        }
    }
}
