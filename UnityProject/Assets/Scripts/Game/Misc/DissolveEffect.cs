using NaughtyAttributes;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    #region Data
    #pragma warning disable 0649
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _effectMaterial;

    [Range(0,1)]
    [SerializeField] private float _cutout;

    private Renderer[] _renderers;
    int shaderProperty;
#pragma warning restore 0649
    #endregion

    #region MonoBehaviour

    private void OnEnable()
    {
        On();

        shaderProperty = Shader.PropertyToID("_cutoff");
    }

    private void OnDisable()
    {
        Off();
    }

    private void Update()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetFloat(shaderProperty, _cutout);
        }
    }
    #endregion

    [Button] void On()
    {
        SetMaterial(_effectMaterial);
    }
    [Button] void Off()
    {
        SetMaterial(_normalMaterial);
    }
    

    private void SetMaterial(Material material)
    {
        _renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in _renderers)
        {
            renderer.materials = new Material[] { material};
        }
    }
}
