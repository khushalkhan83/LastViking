using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class MainIslandChunksActivationController : ActivationObjectsController
    {
        [SerializeField] private GameObject _containerGrass;

        protected override void ChangeActivationObjects(QualityConfig qualityConfig)
        {
            _containerGrass.SetActive(qualityConfig.IsHasGrass);
        }

        protected override void InitActivationObjects(QualityConfig qualityConfig)
        {
            _containerGrass.SetActive(qualityConfig.IsHasGrass);
        } 
    }
}
