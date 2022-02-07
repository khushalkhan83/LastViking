using Extensions;
using Game.AI;
using Game.AI.Behaviours.Kraken;
using UltimateSurvival;
using UnityEngine;
using IDamageable = Game.Models.IDamageable;

namespace Game.QuestSystem.Map.Extra.Kraken
{
    public class KrakenСannonBehaviour : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [Header("Base settings")]
        [SerializeField] private int damage = 100;
        [SerializeField] private KrakenSpawnManager krakenSpawnManager;
        [Header("FX")]
        [SerializeField] private float cameraShakeScale = 5;
        [SerializeField] private GenericShake cameraShakeSettings;
#pragma warning restore 0649
        #endregion
        public void Activate()
        {
            DamageKraken();
            ShakeCameraEffect();
        }

        private void DamageKraken()
        {
            var krakenDamageReciver = krakenSpawnManager.GetComponentInChildren<KrakenDamageReciver>(true);
            gameObject.SafeActivateComponent<Target>();
            (krakenDamageReciver as IDamageable).Damage(damage, gameObject);
        }

        private void ShakeCameraEffect()
        {
            cameraShakeSettings.Shake(cameraShakeScale);
        }

        #region MonoBehaviour
        private void OnValidate()
        {
            if(krakenSpawnManager == null) Debug.LogError("Missing link here");
        }

        #endregion
    }
}