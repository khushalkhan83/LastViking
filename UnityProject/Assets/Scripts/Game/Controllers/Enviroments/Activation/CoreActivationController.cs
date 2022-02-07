using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class CoreActivationController : ActivationObjectsController
    {
        [SerializeField] private GameObject _containerClouds;
        [SerializeField] private GameObject _containerStars;
        [SerializeField] private GameObject _containerMoon;

        protected override void ChangeActivationObjects(QualityConfig qualityConfig)
        {
            _containerClouds.SetActive(qualityConfig.IsHasClouds);
            _containerMoon.SetActive(qualityConfig.IsHasMoon);
            _containerStars.SetActive(qualityConfig.IsHasStars);
        }

        protected override void InitActivationObjects(QualityConfig qualityConfig)
        {
            _containerClouds.SetActive(qualityConfig.IsHasClouds);
            _containerMoon.SetActive(qualityConfig.IsHasMoon);
            _containerStars.SetActive(qualityConfig.IsHasStars);
        }
    }
}
