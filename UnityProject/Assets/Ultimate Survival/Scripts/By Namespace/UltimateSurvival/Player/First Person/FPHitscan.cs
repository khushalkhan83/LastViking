using Game.Audio;
using Game.Models;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
    public class FPHitscan : FPWeaponBase
    {
        public enum ShellSpawnMethod { Auto, OnAnimationEvent }

        #region Data
#pragma warning disable 0649

        [Header("Gun Setup")]

        [SerializeField] private ET.FireMode m_FireMode;

        [Tooltip("The layers that will be affected when you fire.")]
        [SerializeField] private LayerMask m_Mask;

        [Tooltip("If something is farther than this distance threeshold, it will not be affected by the shot.")]
        [SerializeField] private float m_MaxDistance = 150f;

        [Header("Fire Mode.Semi Auto")]

        [Tooltip("The minimum time that can pass between consecutive shots.")]
        [SerializeField] private float m_FireDuration = 0.22f;

        [Header("Fire Mode.Burst")]

        [Tooltip("How many shots will the gun fire when in Burst-mode.")]
        [SerializeField] private int m_BurstLength = 3;

        [Tooltip("How much time it takes to fire all the shots.")]
        [SerializeField] private float m_BurstDuration = 0.3f;

        [Tooltip("The minimum time that can pass between consecutive bursts.")]
        [SerializeField] private float m_BurstPause = 0.35f;

        [Header("Fire Mode.Full Auto")]

        [Tooltip("The maximum amount of shots that can be executed in a minute.")]
        [SerializeField] private int m_RoundsPerMinute = 450;

        [Header("Gun Settings")]

        [Range(1, 20)]
        [Tooltip("The amount of rays that will be sent in the world " +
            "(basically the amount of projectiles / bullets that will be fired at a time).")]
        [SerializeField] private int m_RayCount = 1;

        [Range(0f, 30f)]
        [Tooltip("When NOT aiming, how much the projectiles will spread (in angles).")]
        [SerializeField] private float m_NormalSpread = 0.8f;

        [Range(0f, 30f)]
        [Tooltip("When aiming, how much the projectiles will spread (in angles).")]
        [SerializeField] private float m_AimSpread = 0.95f;

        [SerializeField] private RayImpact m_RayImpact;

        [Header("Audio")]

        [SerializeField] private AudioID _audioID;

        [SerializeField] private AudioID _audioBroken;

        [Header("Effects")]

        [SerializeField] private ParticleSystem m_MuzzleFlash;

        [SerializeField] private GameObject m_Tracer;

        [SerializeField] private Vector3 m_TracerOffset;

        [Header("Shell")]

        [SerializeField] private Rigidbody m_ShellPrefab;

        [SerializeField] private ShellSpawnMethod m_ShellSpawnMethod;

        [SerializeField] private FPHitscanEventHandler m_AnimEventHandler;

        [SerializeField] private Transform m_WeaponRoot;

        [SerializeField] private Vector3 m_ShellSpawnOffset;

        [SerializeField] private Vector3 m_ShellSpawnVelocity = new Vector3(3f, 2f, 0.3f);

        [SerializeField] private float m_ShellSpin = 0.3f;

#pragma warning restore 0649
        #endregion

        private AudioSystem AudioSystem => AudioSystem.Instance;
        private WeaponCoolDownModel WeaponCoolDownModel => ModelsSystem.Instance._weaponCoolDownModel;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        public AudioID AudioID => _audioID;
        public AudioID AudioBroken => _audioBroken;

        private float m_MinTimeBetweenShots;
        private WaitForSeconds m_BurstWait;
        private float FadeOutTime = 0.3f;

        public static bool targetBulletAccessed;

        private bool anyProjectileHit = false;

        private void OnUpdateHandler()
        {
            WeaponCoolDownModel.CoolDownTime -= Time.deltaTime;
            if (WeaponCoolDownModel.CoolDownTime <= 0)
            {
                GameUpdateModel.OnUpdate -= OnUpdateHandler;
            }         
        }

        public override bool TryAttackOnce(Camera camera)
        {
            if ((Time.time - WeaponCoolDownModel.LastFireTime) < m_MinTimeBetweenShots || !IsEnabled)
            {
                return false;
            }

            WeaponCoolDownModel.LastFireTime =  Time.time;

            if (m_FireMode == ET.FireMode.Burst)
            {
                StartCoroutine(C_DoBurst(camera));
            }
            else
            {
                Shoot(camera);
            }

            return true;
        }

        public override bool TryAttackContinuously(Camera camera)
        {
            if (m_FireMode == ET.FireMode.SemiAuto)
            {
                return false;
            }

            return TryAttackOnce(camera);
        }

        protected IEnumerator C_DoBurst(Camera camera)
        {
            for (int i = 0; i < m_BurstLength; i++)
            {
                Shoot(camera);
                yield return m_BurstWait;
            }
        }

        protected void Shoot(Camera camera)
        {
            // Fire sound.
            AudioSystem.PlayOnce(AudioID, transform.position);

            // Muzzle flash.
            if (m_MuzzleFlash)
            {
                m_MuzzleFlash.Play(true);
            }

            anyProjectileHit = false;
            for (int i = 0; i < m_RayCount; i++)
            {
                DoHitscan(camera);
            }

            Attack.Send();

            GameController.Audio.LastGunshot.Set(new Gunshot(transform.position, Player));
        }

        private void TryChangeDurability()
        {
            if(!targetBulletAccessed) return;

            if (!anyProjectileHit && CorrespondingItem != null && CorrespondingItem.TryGetProperty("Durability", out var durability))
            {
                ChangeDurability(durability);
                if (durability.Float.Current <= 0)
                {
                    AudioSystem.PlayOnce(AudioBroken, transform.position);
                }
                CorrespondingItem.SetProperty(durability);
                targetBulletAccessed = false;
                anyProjectileHit = true;
            }
        }

        protected void DoHitscan(Camera camera)
        {
            float spread = Player.Aim.Active ? m_AimSpread : m_NormalSpread;

            var ray = camera.ViewportPointToRay(Vector2.one * 0.5f);
            var spreadVector = camera.transform.TransformVector(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0f));

            ray.direction = Quaternion.Euler(spreadVector) * ray.direction;

            if (Physics.Raycast(ray, out var hitInfo, m_MaxDistance, m_Mask))
            {
                var impulse = m_RayImpact.GetImpulseAtDistance(hitInfo.distance, m_MaxDistance);

                // Do damage.
                var damageableBuilding = hitInfo.collider.GetComponent<DamageReceiver>();
                var damageable = hitInfo.collider.GetComponent<Game.Models.IDamageable>();

                if (damageableBuilding)
                {
                    return;
                }
                else if (damageable != null)
                {
                    targetBulletAccessed = true;
                    HitInfo.SetHitPoint(hitInfo.point);
                    HitInfo.SetHitDirection(ray.direction);
                    HitInfo.SetyKickback(_kickback);

                    var damageValue = m_RayImpact.GetDamageAtDistance(hitInfo.distance, m_MaxDistance);

                    Helpers.DamagableHelper.ShowDamage(damageable,damageValue);

                    Player.PlayerShootSomething = true;
                    damageable.Damage(damageValue, Player.gameObject);
                    Player.PlayerShootSomething = false;
                    OnHitSound(hitInfo);
                    TryChangeDurability();
                }
                // Apply an impact impulse.
                else if (hitInfo.rigidbody)
                {
                    hitInfo.rigidbody.AddForceAtPosition(ray.direction * impulse, hitInfo.point, ForceMode.Impulse);
                }

                // Bullet impact effects.
                if (GameController.SurfaceDatabase)
                {
                    var data = GameController.SurfaceDatabase.GetSurfaceData(hitInfo);
                    data.CreateBulletDecal(hitInfo.point + hitInfo.normal * 0.01f, Quaternion.LookRotation(hitInfo.normal), hitInfo.collider.transform);
                    data.CreateBulletImpactFX(hitInfo.point + hitInfo.normal * 0.01f, Quaternion.LookRotation(hitInfo.normal));
                    data.PlaySound(ItemSelectionMethod.Randomly, SoundType.BulletImpact, 1f, hitInfo.point);
                }
            }

            // Create the tracer if a prefab is assigned.
            if (m_Tracer)
            {
                Instantiate(m_Tracer, transform.position + transform.TransformVector(m_TracerOffset), Quaternion.LookRotation(ray.direction));
            }

            // Create the shell if a prefab is assigned.
            if (m_ShellPrefab && m_ShellSpawnMethod == ShellSpawnMethod.Auto)
            {
                SpawnShell();
            }
        }

        private void Start()
        {
            m_BurstWait = new WaitForSeconds(m_BurstDuration / m_BurstLength);

            if (m_FireMode == ET.FireMode.SemiAuto)
            {
                m_MinTimeBetweenShots = m_FireDuration;
            }
            else if (m_FireMode == ET.FireMode.Burst)
            {
                m_MinTimeBetweenShots = m_BurstDuration + m_BurstPause;
            }
            else
            {
                m_MinTimeBetweenShots = 60f / m_RoundsPerMinute;
            }

            if (m_ShellSpawnMethod == ShellSpawnMethod.OnAnimationEvent && m_AnimEventHandler != null)
            {
                m_AnimEventHandler.AnimEvent_SpawnObject.AddListener(On_AnimEvent_SpawnObject);
            }
        }

        private void OnValidate()
        {
            m_BurstWait = new WaitForSeconds(m_BurstDuration / m_BurstLength);

            if (m_FireMode == ET.FireMode.SemiAuto)
            {
                m_MinTimeBetweenShots = m_FireDuration;
            }
            else if (m_FireMode == ET.FireMode.Burst)
            {
                m_MinTimeBetweenShots = m_BurstDuration + m_BurstPause;
            }
            else
            {
                m_MinTimeBetweenShots = 60f / m_RoundsPerMinute;
            }
        }

        private void On_AnimEvent_SpawnObject(string name)
        {
            if (name == "Shell")
            {
                SpawnShell();
            }
        }

        private void SpawnShell()
        {
            var shell = Instantiate(m_ShellPrefab, m_WeaponRoot.position + m_WeaponRoot.TransformVector(m_ShellSpawnOffset), Random.rotation);
            var shellRB = shell.GetComponent<Rigidbody>();
            shellRB.angularVelocity = new Vector3(Random.Range(-m_ShellSpin, m_ShellSpin), Random.Range(-m_ShellSpin, m_ShellSpin), Random.Range(-m_ShellSpin, m_ShellSpin));
            shellRB.velocity = transform.TransformVector(m_ShellSpawnVelocity);
        }

        protected virtual void OnHitSound(RaycastHit hitInfo)
        {
            var audioIdentifier = hitInfo.collider.GetComponent<AudioIdentifier>();

            if (audioIdentifier)
            {
                AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)], hitInfo.point);
            }
        }
    }
}
