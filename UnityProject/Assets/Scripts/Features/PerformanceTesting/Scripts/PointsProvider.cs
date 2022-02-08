using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsProvider : MonoBehaviour
{
    [SerializeField] private List<Transform> _points;
    
    public int Count => _points.Count;

    private Queue<Transform> _pointsQueue = new Queue<Transform>();

    private void Awake()
    {
        _points.ForEach(p => _pointsQueue.Enqueue(p));
    }
    
    public Transform GetPoint()
    {
        var pointToReturn = _pointsQueue.Dequeue();
        _pointsQueue.Enqueue(pointToReturn);
        return pointToReturn;
    }
}
