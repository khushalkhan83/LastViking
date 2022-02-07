using UnityEngine;

public class GrayscalePostEffect : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _blend;
    [SerializeField] private Shader _shaderGrayscale;

#pragma warning restore 0649
    #endregion

    public float Blend => _blend;
    public Shader ShaderGrayscale => _shaderGrayscale;
    private Material material;

    public void SetBlend(float blend)
    {
        _blend = Mathf.Clamp01(blend);
    }

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(ShaderGrayscale);
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Blend == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetFloat("_Blend", Blend);
        Graphics.Blit(source, destination, material);
    }
}
