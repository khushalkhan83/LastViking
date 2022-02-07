using NaughtyAttributes;
using UnityEngine;

namespace Game.FX
{
    public class BlinkEffect : MonoBehaviour
    {
        [SerializeField] private Material effectMaterial;
        [SerializeField] private float duration = 0.2f;
        
        [SerializeField] private Renderer[] targetRenderers;

        private Material[] defaulMaterials;

        private float timer;
        private bool applyMaterial;
        private bool oldApplyMaterial;

        private void Awake()
        {
            defaulMaterials = new Material[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                defaulMaterials[i] = targetRenderers[i].material;
            }
        }
        
        private void Update()
        {
            timer -= Time.deltaTime;

            applyMaterial = timer > 0;

            if(applyMaterial != oldApplyMaterial)
            {
                for (int i = 0; i < targetRenderers.Length; i++)
                {
                    targetRenderers[i].material = applyMaterial ? effectMaterial : defaulMaterials[i];
                }
            }

            oldApplyMaterial = applyMaterial;
        }

        [Button]
        public void Blink()
        {
            timer = duration;
        }
    }
}