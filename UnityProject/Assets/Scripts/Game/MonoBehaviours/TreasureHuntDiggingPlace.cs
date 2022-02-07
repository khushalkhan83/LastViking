using Game.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

public class TreasureHuntDiggingPlace : MineableObject
{
    [SerializeField]
    GameObject diggedPlace, alivePlace;

    [SerializeField]
    MeshCollider _collider;

    [SerializeField]
    GroundDecal digPlaceDecal, alivePlaceDecal;

    int _index = -1;
    public int index => _index;

    TreasureHuntModel huntModel;

    public void ShowDigged( int myNewIndx, TreasureHuntModel _huntModel)
    {
        _index = myNewIndx;
        huntModel = _huntModel;

        bool isDigged = _huntModel.IsHoleUsed(_index);
        _collider.enabled = !isDigged;
        diggedPlace.SetActive(isDigged);
    }

    public void SetUndiggedVisible(bool isVisible)
    {
        bool isDigged = huntModel.IsHoleUsed(_index);
        alivePlace.SetActive(!isDigged && isVisible);
    }

    public override void OnToolHit(Ray cameraRay, RaycastHit hitInfo, ExtractionSetting[] settings)
    {
        var tool = settings.FirstOrDefault(x => x.ToolID == RequiredToolPurpose);
        if (tool!=null)
            huntModel.TryDigPlace(index);
        //base.OnToolHit(cameraRay, hitInfo, settings);
    }

    public void BakeMesh()
    {
        _collider.enabled = false;
        Mesh collidedMesh = BakeMesh(alivePlace, alivePlaceDecal);
        _collider.sharedMesh = collidedMesh;
        _collider.enabled = true;
        BakeMesh(diggedPlace, digPlaceDecal);
    }

    private Mesh BakeMesh(GameObject meshObject, GroundDecal decal)
    {
        meshObject.SetActive(false);
        decal.ProjectMesh();
        Mesh mesh = Instantiate(decal.bakedMesh);
        List<Vector3> points = new List<Vector3>();
        foreach(Vector3 v3 in mesh.vertices)
        {
            points.Add( transform.InverseTransformPoint(decal.transform.TransformPoint(v3)));
        }
        mesh.SetVertices(points.ToArray());

        return mesh;
    }
}
