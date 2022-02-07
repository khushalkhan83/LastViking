using System.Collections.Generic;
using UnityEngine;

public class ScenePoints : MonoBehaviour
{
    [SerializeField] private List<Transform> _points;

    public IEnumerable<Transform> Points => _points;
}
