using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class TerrainSave : ScriptableObject
{
    [SerializeField] private __TreeInstance[] _trees;

    public TreeInstance[] Trees
    {
        get
        {
            return _trees.Select(x => (TreeInstance)x).ToArray();
        }
        set
        {
            _trees = value.Select(x => new __TreeInstance(x)).ToArray();
        }
    }

    [Serializable]
    public class __TreeInstance
    {

        public __TreeInstance(TreeInstance treeInstance)
        {
            position = treeInstance.position;
            widthScale = treeInstance.widthScale;
            heightScale = treeInstance.heightScale;
            rotation = treeInstance.rotation;
            color = treeInstance.color;
            lightmapColor = treeInstance.lightmapColor;
            prototypeIndex = treeInstance.prototypeIndex;
        }

        public static explicit operator TreeInstance(__TreeInstance obj)
        {
            return new TreeInstance()
            {
                position = obj.position,
                widthScale = obj.widthScale,
                heightScale = obj.heightScale,
                rotation = obj.rotation,
                color = obj.color,
                lightmapColor = obj.lightmapColor,
                prototypeIndex = obj.prototypeIndex
            };
        }

        public Vector3 position;
        public float widthScale;
        public float heightScale;
        public float rotation;
        public Color32 color;
        public Color32 lightmapColor;
        public int prototypeIndex;
    }
}