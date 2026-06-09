using System;
using System.Collections.Generic;
using UnityEngine;

public class ArcRenderer : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject dotPrefab;
    public int poolSize = 50;
    private readonly List<GameObject> _dotPool = new List<GameObject>(); // Stores group of objects that can be reused,
                                                                         // prevents constant instantiation (Dots in this case)
    private GameObject _arrowInstance;
    
    public float spacing = 50.0f;               // The spacing between each dot in arrow's arc
    public float arrowAngleAdjustment = 0;      // Angle correction for the arrowhead
    public int dotsToSkip = 1;                  // Number of dots to skip to give the arrowhead space
    private Vector3 _arrowDirection;            // Holds the position the Arrowhead needs to point from.  
    
    private void Start()
    {
        _arrowInstance = Instantiate(arrowPrefab, transform);
        _arrowInstance.transform.position = Vector3.zero;
        InitializeDotPool(poolSize);
    }

    private void Update()
    {
        var mousePosition = Input.mousePosition;

        mousePosition.z = 0;
        
        var startPosition = transform.position;
        // We need the midpoint in order to show the highest point of the arc
        Vector3 midPoint = CalculateMidPoint(startPosition, mousePosition);
        
        UpdateArc(startPosition, midPoint, mousePosition);
        PositionAndRotateArrow(mousePosition);
    }

    private void UpdateArc(Vector3 start, Vector3 middle, Vector3 end)
    {
        // Determine how many dots to activate from the pool based on arc length
        var numDots = Mathf.CeilToInt(Vector3.Distance(start, end) / spacing);

        for (var i = 0; i < numDots && i < _dotPool.Count; i++)
        {
            // Progress along the curve
            var t = i / (float)numDots;
            t = Mathf.Clamp(t, 0f, 1f); // Ensure t stays between 0 and 1
            
            Vector3 position = QuadraticBezierPoint(start, middle, end, t);

            // Ensures that the arrow does not overlap the last dot
            if (i != numDots - dotsToSkip)
            {
                _dotPool[i].transform.position = position;
                _dotPool[i].SetActive(true);
            }

            if (i == numDots - (dotsToSkip + 1) && i - dotsToSkip + 1 >= 0)
            {
                _arrowDirection = _dotPool[i].transform.position;
            }
        }
        
        // Deactivate unused dots
        for (var i = numDots - dotsToSkip; i < _dotPool.Count; i++)
        {
            if (i > 0)
            {
                _dotPool[i].SetActive(false);
            }
        }
    }

    private void PositionAndRotateArrow(Vector3 position)
    {
        _arrowInstance.transform.position = position;
        var direction = _arrowDirection - position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += arrowAngleAdjustment;
        _arrowInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private static Vector3 CalculateMidPoint(Vector3 start, Vector3 end)
    {
        var midPoint = (start + end) / 2;
        var arcHeight = Vector3.Distance(start, end) / 3f; // 
        midPoint.y += arcHeight;
        return midPoint;
    }

    private static Vector3 QuadraticBezierPoint(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        
        var point = uu * start;
        point += 2 * u * t * control;
        point += tt * end;
        return point;
    }

    private void InitializeDotPool(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var dot = Instantiate(
                dotPrefab,                  // Create dotPrefab
                Vector3.zero,               // At local position (0,0,0)
                Quaternion.identity,        // With no rotation
                transform);                 // As a child of THIS object
            dot.SetActive(false);
            _dotPool.Add(dot);
        }
    }
}
