using Game.AI;
using Game.Audio;
using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
    /// <summary>
    /// 
    /// </summary>
    public class FPCrossBow : FPWeaponBase
    {

        #region Data
#pragma warning disable 0649

        [Header("Bow Setup")]

        [SerializeField] private LayerMask m_Mask;

        [SerializeField] private float m_MaxDistance = 50f;

        [Range(0f, 30f)]
        [Tooltip("When NOT aiming, how much the projectiles will spread (in angles).")]
        [SerializeField] private float _normalSpread = 2f;
        [SerializeField] private float _aimSpread = 0.8f;

        [Header("Bow Settings")]

        [SerializeField] private float m_MinTimeBetweenShots = 1f;

        [Header("Arrow")]

        [SerializeField] private ShaftedProjectile m_ArrowPrefab;

        [SerializeField] private Vector3 m_SpawnOffset;

        [Header("Audio")]

        [SerializeField] private AudioSource m_AudioSource;

        [SerializeField] private SoundPlayer m_ReleaseAudio;

        [SerializeField] private SoundPlayer m_StretchAudio;

        [SerializeField] private AudioID _audioBroken;


#pragma warning restore 0649
        #endregion
        public AudioID AudioBroken => _audioBroken;

        public AudioSystem AudioSystem => AudioSystem.Instance;

        private float m_NextTimeCanShoot;

        private const float AIMING_DURATION = 0.4f;
        Coroutine ShoootCoroutine;
        public override bool TryAttackOnce(Camera camera)
        {
            if (Time.time < m_NextTimeCanShoot)
                return false;

            m_ReleaseAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource, 1f);

            var isAiming = Player.Aim.Active;
            var timeShoot = m_MinTimeBetweenShots;

            if (!isAiming)
            {
                timeShoot += AIMING_DURATION;
            }

            m_NextTimeCanShoot = Time.time + timeShoot;
            if (ShoootCoroutine != null)
            {
                StopCoroutine(ShoootCoroutine);
            }
            ShoootCoroutine = StartCoroutine(Shooot(camera));


            return true;
        }

        protected void Awake()
        {
            Player.Aim.AddStartTryer(OnTryStart_Aim);
        }

        private bool OnTryStart_Aim()
        {
            bool canStart = Time.time > m_NextTimeCanShoot || !IsEnabled;

            if (canStart && IsEnabled)
                m_StretchAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);

            return canStart;
        }

        private IEnumerator Shooot(Camera camera)
        {
            var isAiming = Player.Aim.Active;
            SpawnArrow(camera);
            Attack.Send();
            if (!isAiming)
            {
                yield return new WaitForSeconds(AIMING_DURATION);
                Player.Aim.ForceStop();
            }
            else
            {
                yield return new WaitForSeconds(AIMING_DURATION * 2.25f);
                Player.Aim.ForceStop();
                Player.Aim.ForceStart();
            }
            ShoootCoroutine = null;
        }

        private void SpawnArrow(Camera camera)
        {
            if (!m_ArrowPrefab)
            {
                Debug.LogErrorFormat("[{0}.FPBow] - No arrow prefab assigned in the inspector! Please assign one.", name);
                return;
            }

            var ray = camera.ViewportPointToRay(Vector3.one * 0.5f);

            float spread = Player.Aim.Active ? -_aimSpread : _normalSpread;
            var spreadVector = camera.transform.TransformVector(new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0f));

            ray.direction = Quaternion.Euler(spreadVector) * ray.direction;

            // Getting the target point.
            Vector3 hitPoint;
            if (Physics.Raycast(ray, out var hitInfo, m_MaxDistance, m_Mask))
            {
                hitPoint = hitInfo.point;
            }
            else
            {
                hitPoint = camera.transform.position + ray.direction.normalized * m_MaxDistance;
            }

            var position = transform.position + camera.transform.TransformVector(m_SpawnOffset);
            var rotation = Quaternion.LookRotation(hitPoint - position);

            Instantiate(m_ArrowPrefab, position, rotation).Launch(Player.GetComponent<Target>(), _kickback);

            StartCoroutine(ArrowAccess());
        }

        private IEnumerator ArrowAccess()
        {
            yield return new WaitForSeconds(0.1f);
            if (ShaftedProjectile.targetAccessed)
            {
                if (CorrespondingItem.TryGetProperty("Durability", out var durability))
                {
                    ChangeDurability(durability);
                    if (durability.Float.Current <= 0)
                    {
                        AudioSystem.PlayOnce(AudioBroken, transform.position);
                    }
                    CorrespondingItem.SetProperty(durability);
                    ShaftedProjectile.targetAccessed = false;
                }
            }
        }
    }
}
