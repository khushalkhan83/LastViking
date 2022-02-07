using Game.AI;
using Game.AI.BehaviorDesigner;
using Game.Audio;
using Game.Models;
using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
   
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ShaftedProjectile : MonoBehaviour
    {
        public enum ProjectileOwner
        {
            Player,
            Enemy
        }

        public event System.Action<Target> OnKillTarget;

        #region Data
#pragma warning disable 0649

        [Header("Setup")]

        [SerializeField] private Transform m_Model;

        [SerializeField] private Transform m_Pivot;

        [SerializeField] private LayerMask m_Mask;

        [SerializeField] private float m_MaxDistance = 2f;

        [Header("Launch")]

        [SerializeField] private float m_LaunchSpeed = 50f;

        [Header("Damage")]

        [SerializeField] private float m_MaxDamage = 100f;
        [SerializeField] private bool m_useConfig;
        [SerializeField] private EnemyConfig m_config;

        [Tooltip("How the damage changes, when the speed gets lower.")]
        [SerializeField]
        private AnimationCurve m_DamageCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.8f, 0.5f),
            new Keyframe(1f, 0f));

        [Header("Penetration")]

        [Tooltip("The speed under which the projectile will not penetrate objects.")]
        [SerializeField] private float m_PenetrationThreeshold = 20f;

        [SerializeField] private float m_PenetrationOffset = 0.2f;

        [SerializeField] private Vector2 m_RandomRotation;

        [Header("Twang")]

        [SerializeField] private float m_TwangDuration = 1f;

        [SerializeField] private float m_TwangRange = 18f;

        [Header("Audio")]

        [SerializeField] private AudioSource m_AudioSource;

        [SerializeField] private SoundPlayer m_HitAudio;

        [SerializeField] private SoundPlayer m_TwangAudio;

        [SerializeField] private SoundType m_PenetrationType;

        [SerializeField] private ProjectileOwner owner = ProjectileOwner.Player;

#pragma warning restore 0649
        #endregion

        private EntityEventHandler m_EntityThatLaunched;
        private Collider m_Collider;
        private Rigidbody m_Rigidbody;
        private bool m_Done;
        public static bool targetAccessed;
        private bool m_Launched;
        private float m_kickback;
        private SurfaceDatabaseModel SurfaceDatabaseModel => ModelsSystem.Instance.surfaceDatabaseModel;
        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;

        private float MaxDamage => m_useConfig && m_config != null ? m_config.AttackDamage : m_MaxDamage;

        public Target From { get; private set; }
        public float LaunchSpeed => m_LaunchSpeed;
        public AudioSystem AudioSystem => AudioSystem.Instance;
        public ProjectileOwner Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public void Launch(Target targetThatLaunched, float kickback)
        {
            m_kickback = kickback;
            if (m_Launched)
            {
                Debug.LogWarningFormat(this, "Already launched this projectile!", name);
                return;
            }

            m_Rigidbody.velocity = transform.forward * m_LaunchSpeed;
            m_Launched = true;
            From = targetThatLaunched;
        }

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Rigidbody = GetComponent<Rigidbody>();

            m_Collider.enabled = false;
        }

        private void FixedUpdate()
        {
            if (m_Done)
                return;

            RaycastHit hitInfo;
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out hitInfo, m_MaxDistance, m_Mask))
            {
                if (!IsCanHitTarget(hitInfo))
                {
                    return;
                }

                var data = SurfaceDatabaseModel.SurfaceDatabase.GetSurfaceData(hitInfo);

                float currentSpeed = m_Rigidbody.velocity.magnitude;

                if (currentSpeed >= m_PenetrationThreeshold && data != null)
                {
                    transform.SetParent(hitInfo.collider.transform, true);

                    // Stick the projectile in the object.
                    transform.position = hitInfo.point + transform.forward * m_PenetrationOffset;
                    m_Pivot.localPosition = Vector3.back * m_PenetrationOffset;

                    data.PlaySound(ItemSelectionMethod.RandomlyButExcludeLast, m_PenetrationType, 1f, m_AudioSource);

                    m_Model.SetParent(m_Pivot, true);

                    float impulse = m_Rigidbody.mass * currentSpeed;

                    float damage = MaxDamage * m_DamageCurve.Evaluate(1f - currentSpeed / m_LaunchSpeed);

                    var damageable = hitInfo.collider.GetComponentInParent<Game.Models.IDamageable>();

                    var target = hitInfo.transform.GetComponentInParent<Target>();
                    if(Owner == ProjectileOwner.Enemy && target != null)
                    {
                        damage = EnemiesAttackModificators.GetDamageForTarget(target.ID, damage);
                    }

                    if (damageable != null)
                    {
                        targetAccessed = true;
                        if(From != null)
                        {
                            var hit = From.GetComponent<HitInfo>();
                            hit.SetHitPoint(hitInfo.point);
                            hit.SetHitDirection(ray.direction);
                            hit.SetyKickback(m_kickback);
                        }
                        
                        Helpers.DamagableHelper.ShowDamage(damageable,damage);
                        OnHitSound(hitInfo);
                        if(From != null)
                        {
                            damageable.Damage(damage, From.gameObject);
                        }
                        else
                        {
                            damageable.Damage(damage, null);
                        }

                        if(target != null)
                        {
                            bool targetIsPlayer = target.ID == TargetID.Player;
                            IHealth healthInTarget = targetIsPlayer ? PlayerHealthModel : target.GetComponentInParent<IHealth>();
                            if (healthInTarget != null && healthInTarget.IsDead)
                            {
                                OnKillTarget?.Invoke(target);
                            }
                        }
                    }

                    // If the object is not damageable, but it's a rigidbody, apply the impact impulse.
                    else if (hitInfo.rigidbody)
                        hitInfo.rigidbody.AddForceAtPosition(transform.forward * impulse, transform.position, ForceMode.Impulse);

                    //:( need rewrite all this sh***
                    if (isActiveAndEnabled)
                    {
                        StartCoroutine(C_DoTwang());
                    }

                    var hitbox = hitInfo.collider.GetComponent<HitBox>();
                    if (hitbox)
                    {
                        var cols = Physics.OverlapSphere(transform.position, 1.5f, m_Mask);
                        for (int i = 0; i < cols.Length; i++)
                        {
                            var colHitbox = cols[i].GetComponent<HitBox>();
                            if (colHitbox)
                                Physics.IgnoreCollision(colHitbox.Collider, m_Collider);
                        }
                    }
                    Physics.IgnoreCollision(m_Collider, hitInfo.collider);
                    m_Rigidbody.isKinematic = true;
                }
                else
                {
                    m_HitAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);
                    m_Rigidbody.isKinematic = false;
                }

                m_Collider.enabled = true;
                m_Collider.isTrigger = true;

                m_Done = true;
            }
        }

        private bool IsCanHitTarget(RaycastHit hitInfo)
        {
            if (Owner == ProjectileOwner.Player)
            {
                var damageableBuilding = hitInfo.collider.GetComponentInParent<DamageReceiver>();
                if (damageableBuilding != null)
                    return false;
            }
            else if(Owner == ProjectileOwner.Enemy)
            {
                var enemyDamageReciver = hitInfo.collider.GetComponentInParent<EnemyDamageReciver>();
                if (enemyDamageReciver != null)
                    return false;
            }
            return true;
        }

        private IEnumerator C_DoTwang()
        {
            float stopTime = Time.time + m_TwangDuration;
            float range = m_TwangRange;
            float currentVelocity = 0f;

            Quaternion randomRotation = Quaternion.Euler(new Vector2(
                Random.Range(-m_RandomRotation.x, m_RandomRotation.x),
                Random.Range(-m_RandomRotation.y, m_RandomRotation.y)));

            while (Time.time < stopTime)
            {
                m_Pivot.localRotation = randomRotation * Quaternion.Euler(Random.Range(-range, range), Random.Range(-range, range), 0f);
                range = Mathf.SmoothDamp(range, 0f, ref currentVelocity, stopTime - Time.time);

                yield return null;
            }
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