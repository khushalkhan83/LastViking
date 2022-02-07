using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class MainIslandActivationController : ActivationObjectsController
    {
        [SerializeField] private GameObject _containerButterflies;

        protected override void ChangeActivationObjects(QualityConfig qualityConfig)
        {
            _containerButterflies.SetActive(qualityConfig.IsHasButterflies);
        }

        protected override void InitActivationObjects(QualityConfig qualityConfig)
        {
            _containerButterflies.SetActive(qualityConfig.IsHasButterflies);
        } 
    }
}
