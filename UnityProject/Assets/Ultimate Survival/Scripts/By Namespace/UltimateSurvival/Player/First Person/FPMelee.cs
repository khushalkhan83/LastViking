using Game.Audio;
using Game.Models;
using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
    public class FPMelee : FPWeaponBase
    {
        public enum HitInvokeMethod
        {
            ByTimer,
            ByAnimationEvent
        }


        /// <summary>True when an object was in range (subscribe to this instead of the regular "Attack" message, when dealing with melee weapons).</summary>
        public Message<bool> MeleeAttack { get { return m_MeleeAttack; } }
        public Message Miss { get { return m_Miss; } }
        public Message Hit { get { return m_Hit; } }
        public float MaxReach { get { return m_MaxReach; } }
        public float DamagePerHit { get { return m_DamagePerHit; } }
        public float TimeBetweenAttacks { get { return m_TimeBetweenAttacks; } }
        private HintsModel HintsModel => ModelsSystem.Instance._hintsModel;
        protected PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        #region Data
#pragma warning disable 0649

        [Header("Melee Setup")]

        [Tooltip("The animation event handler - that picks up animation events from Mecanim.")]
        [SerializeField] private FPMeleeEventHandler m_EventHandler;

        [Header("Melee Settings")]

        [Tooltip("From how far can this object hit stuff?")]
        [SerializeField] protected float m_MaxReach = 0.5f;

        [Tooltip("Useful for limiting the number of hits you can do in a period of time.")]
        [SerializeField] protected float m_TimeBetweenAttacks = 0.85f;

        [Range(0f, 1000f)]
        [SerializeField] private float m_DamagePerHit = 15f;

        [SerializeField] private AudioID _audioBroken;

#pragma warning restore 0649
        #endregion

        public AudioID AudioBroken => _audioBroken;

        private Message<bool> m_MeleeAttack = new Message<bool>();
        private Message m_Miss = new Message();
        private Message m_Hit = new Message();
        protected float m_NextUseTime;

        public AudioSystem AudioSystem => AudioSystem.Instance;

        virtual protected bool IsCanHitBuilding { get; set; } = true;
        virtual protected bool IsCanHitDamageble { get; set; } = true;

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        protected IEnumerator AttackSound()
        {
            yield return new WaitForSeconds(0.1f);
            AudioSystem.PlayOnce(AudioID.Whoosh02);
        }

        public override bool TryAttackOnce(Camera camera)
        {
            if (Time.time < m_NextUseTime)
            {
                return false;
            }

            //var  (Player.hitZone.target as MonoBehaviour);

            var raycastData = Player.RaycastData.Value;

            // Send the melee specific attack message.
            if (raycastData)
            {
                bool objectIsClose = raycastData.HitInfo.distance < m_MaxReach;
                MeleeAttack.Send(objectIsClose);
            }
            else
            {
                MeleeAttack.Send(false);
            }

            StartCoroutine(AttackSound());
            // Send the regular attack message.
            Attack.Send();

            m_NextUseTime = Time.time + m_TimeBetweenAttacks;

            return true;
        }

        public override bool TryAttackContinuously(Camera camera) => TryAttackOnce(camera);

        protected virtual void Start()
        {
            m_EventHandler.Hit.AddListener(On_Hit);
            m_EventHandler.Woosh.AddListener(On_Woosh);
        }

        protected void OnHitBuilding()
        {
            var raycastData = Player.RaycastData.Value;
            var damageableBuilding = raycastData.GameObject?.GetComponent<DamageReceiver>();
            var damageable = raycastData.GameObject?.GetComponent<Game.Models.IDamageable>();
            var healthBuilding = raycastData.GameObject?.GetComponentInParent<BuildingHealthModel>();

            if (damageableBuilding.ID == PlayerTargetItem.Buildings)
            {
                if (damageable != null)
                {
                    HitInfo.SetHitPoint(raycastData.HitInfo.point);
                    HitInfo.SetHitDirection(-raycastData.HitInfo.normal);
                    HitInfo.SetyKickback(_kickback);
                    var damageValue = healthBuilding.HealthMax/3 + 1;

                    Helpers.DamagableHelper.ShowDamage(damageable,damageValue);

                    PlayerEventHandler.PlayerAttackBuilding = true;
                    damageable.Damage(damageValue, Player.gameObject);
                    PlayerEventHandler.PlayerAttackBuilding = false;
                }
            }

            Hit.Send();
        }

        protected virtual void On_Hit()
        {
            var raycastData = Player.RaycastData.Value;

            if (raycastData == null || raycastData.GameObject == null)
            {
                return;
            }

            GameObject hitObj = raycastData.GameObject;

            OnHitSound();

            var damageableBuilding = hitObj.GetComponent<DamageReceiver>();
            if (damageableBuilding)
            {
                if (IsCanHitBuilding)
                {
                    OnHitBuilding();
                }
            }
            else 
            {
                var damageable = hitObj.GetComponent<Game.Models.IDamageable>();
                if (damageable != null && IsCanHitDamageble)
                {
                    HitInfo.SetHitPoint(raycastData.HitInfo.point);
                    HitInfo.SetHitDirection(-raycastData.HitInfo.normal);
                    HitInfo.SetyKickback(_kickback);

                    Helpers.DamagableHelper.ShowDamage(damageable,m_DamagePerHit);

                    damageable.Damage(m_DamagePerHit, Player.gameObject);
                    OnDecreaseDurability();
                }

                HintsModel.TryShowMinableToolHint(hitObj);
            }

            Hit.Send();
        }

        protected virtual void OnDecreaseDurability()
        {
            if (CorrespondingItem != null && CorrespondingItem.TryGetProperty("Durability", out var durability))
            {
                ChangeDurability(durability);
                if (durability.Float.Current <= 0)
                {
                    AudioSystem.PlayOnce(AudioBroken, transform.position);
                }
                CorrespondingItem.SetProperty(durability);
            }
        }

        protected virtual void OnHitSound()
        {
            var raycastData = Player.RaycastData.Value;

            if(raycastData == null || raycastData.GameObject == null) return;

            var audioIdentifier = raycastData.GameObject.GetComponent<AudioIdentifier>();

            if (audioIdentifier)
            {
                AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)], raycastData.HitInfo.point);
            }
        }

        protected virtual void On_Woosh() => Miss.Send();
    }
}
