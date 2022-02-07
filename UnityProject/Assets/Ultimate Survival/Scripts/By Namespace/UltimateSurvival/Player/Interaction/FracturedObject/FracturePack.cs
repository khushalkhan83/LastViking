using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FracturePack
{
    private static List<Tuple<FracturePieceBehaviour, RaycastHit>> PiecesRaycastBuffer { get; } = new List<Tuple<FracturePieceBehaviour, RaycastHit>>(10);

    private GameObject Instance { get; }

    public FracturePieceBehaviour[] PieceBehaviours { get; }

    public int TotalBrokenPieces { get; private set; }
    public int TotalPieces => Pieces.Length;

    public FracturePack(GameObject prefab)
    {
        AssociatedPrefab = prefab;

        Instance = UnityEngine.Object.Instantiate(prefab);
        Instance.name = prefab.name;

        PieceBehaviours = Instance.GetComponentsInChildren<FracturePieceBehaviour>();

        foreach (var pieceBehaviour in PieceBehaviours)
        {
            pieceBehaviour.BrokenStateChange += isBroken => TotalBrokenPieces = isBroken ? ++TotalBrokenPieces : --TotalBrokenPieces;
        }
    }

    public GameObject AssociatedPrefab { get; }

    public FracturePieceBehaviour[] Pieces => PieceBehaviours;

    public bool Active
    {
        get => Instance.activeSelf;
        set => Instance.SetActive(value);
    }

    public FracturePieceBehaviour RaycastPieces(Ray ray)
    {
        PiecesRaycastBuffer.Clear();

        foreach (var pieceBehaviour in Pieces)
        {
            if (pieceBehaviour.Raycast(ray, out var raycastHit))
            {
                PiecesRaycastBuffer.Add(Tuple.Create(pieceBehaviour, raycastHit));
            }
        }

        var closestHit = PiecesRaycastBuffer.OrderBy(item => item.Item2.distance).FirstOrDefault();

        PiecesRaycastBuffer.Clear();

        return closestHit?.Item1;
    }

    public void OnReturnToPool(Transform iTransform)
    {
        Active = false;

        SetParent(iTransform);

        // Set fracture pieces to default state.
        foreach (var pieceBehaviour in Pieces)
        {
            pieceBehaviour.Reset();
        }
    }

    public void OnProvided(Transform parent)
    {
        SetParent(parent);

        // Set fracture pieces to default state.
        foreach (var pieceBehaviour in Pieces)
        {
            pieceBehaviour.Reset();
        }
    }

    private void SetParent(Transform parent)
    {
        // Set Parent and zero local transform.
        Instance.transform.parent = parent;
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localEulerAngles = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
    }
}