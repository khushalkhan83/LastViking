using Core.StateMachine;
using Game.Models;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken
{
    public class KrakenDamageReciver : MonoBehaviour, IDamageable, UltimateSurvival.IHitReciver
    {
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;
        
        #region Data
#pragma warning disable 0649
        [SerializeField] private int _goldForKill;
        [SerializeField] private int _blueprintForKill;
        [SerializeField] private EffectBase[] _effectsOnDeath;
        [SerializeField] Vector3 targetShift = new Vector3(0f, 1.45f, 0.3f);

#pragma warning restore 0649
        #endregion

        private float random;
        private IHealth _health;

        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());
        public EffectBase[] EffectsOnDeath => _effectsOnDeath;

        #region MonoBehaviour
        private void Awake()
        {
            random = UnityEngine.Random.Range(0, 100);
        }
        
        #endregion

        void IDamageable.Damage(float value, GameObject from)
        {
            var fromTarget = from.GetComponent<Target>();
            Health.AdjustHealth(-value);
            FirstKrakenModel.SetKrakenMaxHealth(Health.HealthMax);
            FirstKrakenModel.SetKrakenHealth(Health.Health);

            if (!Health.IsDead) return;

            if (fromTarget.ID == TargetID.Player) //TODO: add statistic
            {
                StatisticsModel.Kill();
                StatisticsModel.KillAnimal();
            }
            foreach (var effect in EffectsOnDeath)
            {
                effect.Apply();
            }

            if (random <= 50f)
            {
                CoinObjectsModel.SpawnAtPosition(_goldForKill, transform.position, transform.position + Vector3.up, 2f, AnimalID.Kraken.ToString());
            }
            else
            {
                BlueprintObjectsModel.SpawnAtPosition(_blueprintForKill, transform.position, transform.position + Vector3.up, 2f);
            }

            AnimalsModel.TargetKillAnimal(from.GetComponent<Target>(), AnimalID.Kraken);
        }

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(targetShift);
        }
    }
}
