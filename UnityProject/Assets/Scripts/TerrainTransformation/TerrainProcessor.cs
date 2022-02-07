using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainProcessor : MonoBehaviour
{
    [Serializable]
    public class ReplasmentData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _id;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _container;
        [SerializeField] private float _positionRadiusRandom;
        [SerializeField] private Vector3 _rotationRandom;
        [SerializeField] private float _scaleMin;
        [SerializeField] private float _scaleMax;

#pragma warning restore 0649
        #endregion

        public int ID => _id;
        public float PositionRadiusRandom => _positionRadiusRandom;
        public float ScaleMin => _scaleMin;
        public float ScaleMax => _scaleMax;
        public Vector3 RotationRandom => _rotationRandom;
        public GameObject Prefab => _prefab;
        public Transform Container => _container;

        public Vector3 GetPositionOffsetRandom()
        {
            var value = Random.Range(0, PositionRadiusRandom);
            return Vector3.one * value;
        }

        public Quaternion GetRotationOffsetRandom()
        {
            return Quaternion.Euler(Random.Range(0, RotationRandom.x), Random.Range(0, RotationRandom.y), Random.Range(0, RotationRandom.z));
        }

        public Vector3 GetScaleCoeficientRandom()
        {
            var value = Random.Range(ScaleMin, ScaleMax);
            return Vector3.one * value;
        }
    }

    #region Data
#pragma warning disable 0649

    [SerializeField] private ReplasmentData[] _data;

#pragma warning restore 0649
    #endregion

    public IEnumerable<ReplasmentData> Data => _data;

    //

    public Terrain Terrain => GetComponent<Terrain>();

    //

    public TreeInstance[] SaveData { get; private set; }

    //

    public void ReplaseObjects()
    {
        var other = new List<TreeInstance>();
        foreach (var item in Terrain.terrainData.treeInstances)
        {
            var replace = Data.FirstOrDefault(x => x.ID == item.prototypeIndex);
            if (replace != null)
            {
                Instantiate(item, replace);
            }
            else
            {
                other.Add(item);
            }
        }

        Terrain.terrainData.treeInstances = other.ToArray();
    }

    private void Instantiate(TreeInstance item, ReplasmentData replace)
    {
        var positionWorld = Vector3.Scale(item.position, Terrain.terrainData.size);
        var positionOffset = Vector3.Scale(Random.insideUnitSphere.normalized, replace.GetPositionOffsetRandom());
        var position = positionWorld + positionOffset;
        position.y = Terrain.SampleHeight(position);

        var rotation = replace.Prefab.transform.rotation * replace.GetRotationOffsetRandom();
        var scalePercent = replace.GetScaleCoeficientRandom();

        var instance = Instantiate(replace.Prefab, position, rotation, replace.Container);
        instance.transform.localScale = Vector3.Scale(instance.transform.localScale, scalePercent);

    }

    public void Clear()
    {
        Terrain.terrainData.treeInstances = new TreeInstance[0];
    }
}
