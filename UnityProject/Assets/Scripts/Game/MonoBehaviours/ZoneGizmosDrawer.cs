using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGizmosDrawer : MonoBehaviour
{
    public enum ZoneType
    {
        None,
        Sphere, 
        Box
    }

    #region Data
#pragma warning disable 0649

#if UNITY_EDITOR

    [SerializeField] private Color _gizmosColor = new Color (1, 1, 1, 0.5f);
    
#endif

#pragma warning restore 0649
    #endregion

    public SphereCollider SphereCollider { private set; get; }
    public BoxCollider BoxCollider { private set; get; }

    public ZoneType TypeZone { private set; get; }

    private void OnValidate()
    {
        SphereCollider = GetComponent<SphereCollider>();
        BoxCollider = GetComponent<BoxCollider>();

        if (SphereCollider) TypeZone = ZoneType.Sphere;
        else if (BoxCollider) TypeZone = ZoneType.Box;
        else TypeZone = ZoneType.None;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        switch (TypeZone)
        {
            case ZoneType.None:
                Gizmos.DrawSphere(Vector3.zero, 1);
                break;
            case ZoneType.Sphere:
                Gizmos.DrawSphere(Vector3.zero + SphereCollider.center, SphereCollider.radius);
                break;
            case ZoneType.Box:
                Gizmos.DrawCube(Vector3.zero + BoxCollider.center, BoxCollider.size);
                break;
            default:
                break;
        }
    }
#endif
}
