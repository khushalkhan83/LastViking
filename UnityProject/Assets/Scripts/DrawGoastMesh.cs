using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGoastMesh : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
    [SerializeField] private Mesh _mesh;

    private void OnDrawGizmos()
    {
        if (_mesh)
        {
            Gizmos.color = _colorGost;
            Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, transform.localScale);
        }
    }
#endif
}
