using Game.Models;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken
{
    public class KrakenHealthSetter : MonoBehaviour
    {
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;
        private IHealth health;

        // called by InitableWithEvents
        public void Init()
        {
            health = GetComponentInChildren<IHealth>();
            health.SetHealth(FirstKrakenModel.Health);

            // TODO: remove spaghetti (set health max on IHealth component, KrakenHealth with max health, KrakenDamageReciver set health, KillKrakenConditionData max health)
            // health.HealthMax(FirstKrakenModel.MaxHealth);
        }
    }
}
