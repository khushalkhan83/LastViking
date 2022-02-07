using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDecal : MonoBehaviour
{
    [SerializeField]
    float height = 0.1f;

    [SerializeField]
    Mesh _bakedMesh;

    public Mesh bakedMesh => _bakedMesh;

    public void ProjectMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter Not Found at "+ name);
            return;
        }

        _bakedMesh = Instantiate( meshFilter.sharedMesh);
        if (_bakedMesh==null)
        {
            Debug.LogError("Mesh Not Found at " + name);
            return;
        }

        List<Vector3> newVertexList = new List<Vector3>();
        foreach(Vector3 vertex in _bakedMesh.vertices)
        {
            RaycastHit raycastHit;
            
            if (Physics.Raycast(transform.TransformPoint(vertex)+Vector3.up*3f, Vector3.down,out raycastHit,10f))
            {
                Vector3 newPos = raycastHit.point + Vector3.up * height;
                newPos =  transform.InverseTransformPoint(newPos);
                newVertexList.Add(newPos);
            }
            else
            {
                newVertexList.Add(vertex);
                Debug.Log("Projection Error, no hit");
            }
        }

        _bakedMesh.SetVertices(newVertexList.ToArray());

        meshFilter.mesh = _bakedMesh;
    }
}
