using UnityEngine;

namespace Weather.Extenral.SkyboxCubemapExtended
{
    [ExecuteInEditMode]
    public class SkyBoxCubemapExtendedControl : MonoBehaviour
    {
        [SerializeField] private bool active;

        [Header("Properties")]
        [SerializeField] private float fogFill;
        [SerializeField] private float cubemapPosition;
        [SerializeField] private float cubemapExposure;

        void Update()
        {
            if(!active) return;

            RenderSettings.skybox.SetFloat("_FogFill",fogFill);
            RenderSettings.skybox.SetFloat("_CubemapPosition",cubemapPosition);
            RenderSettings.skybox.SetFloat("_Exposure",cubemapExposure);
        }
    }
}