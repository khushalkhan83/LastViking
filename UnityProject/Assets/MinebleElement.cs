using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

public class MinebleElement : MonoBehaviour
{
    [SerializeField] private OutLineMinableObjectID outLineMinableObjectID;
    [SerializeField] private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<MinebleFractureObject> minableFractures = new List<MinebleFractureObject>();

    public OutLineMinableObjectID OutLineMinableObjectID => outLineMinableObjectID;
    public List<MeshRenderer> MeshRenderers => meshRenderers;
    public List<MinebleFractureObject> MinableFractures => minableFractures;

    private MinebleElementsModel MinebleElementsModel => ModelsSystem.Instance._minebleElementsModel;


    private void OnEnable()
    {
        MinebleElementsModel.RegisterMinable(outLineMinableObjectID, this);
    }

    private void OnDisable()
    {
        MinebleElementsModel.UnRegisterMinable(outLineMinableObjectID, this);
    }
}
