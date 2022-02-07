using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class StatsEffectModel : MonoBehaviour
    {
        public Transform ViewContainer { get; private set; }

        public event Action<SavableItem, Vector3> OnShowStatsEffect;
        public event Action OnHideStatsEffect;

        public void SetViewContainer(Transform container) => ViewContainer = container;

        public void AddStatsEffect(SavableItem item, Vector3 position) => OnShowStatsEffect?.Invoke(item, position);

        public void HideStatsEffect() => OnHideStatsEffect?.Invoke();
    }
}
