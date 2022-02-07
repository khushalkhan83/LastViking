using Game.AI;
using Game.Audio;
using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
    public class FPSpear : FPWeaponBase
    {

        #region Data
#pragma warning disable 0649

        [Header("Setup")]

        [SerializeField] private LayerMask m_Mask;

        [SerializeField] private float m_MaxDistance = 50f;

        [Header("Settings")]

        [SerializeField] private float m_MinTimeBetweenThrows = 1.5f;

        [Header("Object To Throw")]

        [SerializeField] private ShaftedProjectile m_SpearPrefab;

        [SerializeField] private Vector3 m_SpawnOffset;

        [SerializeField] private float m_SpawnDelay = 0.3f;

        [Header("Audio")]

        [SerializeField] private AudioSource m_AudioSource;

        [SerializeField] private SoundPlayer m_ThrowAudio;

        [SerializeField] private AudioID _audioBroken;

#pragma warning restore 0649
        #endregion

        public AudioID AudioBroken => _audioBroken;

        public AudioSystem AudioSystem => AudioSystem.Instance;

        private float m_NextTimeCanThrow;

        public bool CanThrow => Time.time > m_NextTimeCanThrow;

        private const float AIMING_DURATION = 0.2f;
        Coroutine ShoootCoroutine;

        public override bool TryAttackOnce(Camera camera)
        {
            if (!CanThrow)
            {
                return false;
            }

            m_ThrowAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource, 1f);
            if (ShoootCoroutine != null)
            {
                StopCoroutine(ShoootCoroutine);
            }
            ShoootCoroutine = StartCoroutine(C_ThrowWithDelay(camera, m_SpawnDelay));

            var isAiming = Player.Aim.Active;
            var timeShoot = m_MinTimeBetweenThrows;

            if (!isAiming)
            {
                timeShoot += AIMING_DURATION;
            }

            m_NextTimeCanThrow = Time.time + timeShoot;

            return true;
        }

        private void Start()
        {
            Player.Aim.AddStartTryer(OnTryStart_Aim);
        }

        private bool OnTryStart_Aim()
        {
            return !IsEnabled || CanThrow;
        }

        private IEnumerator C_ThrowWithDelay(Camera camera, float delay)
        {
            if (!m_SpearPrefab)
            {
                Debug.LogErrorFormat(this, "The spear prefab is not assigned in the inspector!.", name);
                yield break;
            }

            var isAiming = Player.Aim.Active;
            if (!isAiming)
            {
                Player.Aim.ForceStart();
                yield return new WaitForSeconds(AIMING_DURATION);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
            Attack.Send();

            var ray = camera.ViewportPointToRay(Vector3.one * 0.5f);

            // Get the target point.
            Vector3 hitPoint;
            if (Physics.Raycast(ray, out var hitInfo, m_MaxDistance, m_Mask))
            {
                hitPoint = hitInfo.point;
            }
            else
            {
                hitPoint = camera.transform.position + camera.transform.forward * m_MaxDistance;
            }

            var position = transform.position + camera.transform.TransformVector(m_SpawnOffset);
            var rotation = Quaternion.LookRotation(hitPoint - position);

            var spearObject = Instantiate(m_SpearPrefab, position, rotation);
            spearObject.Launch(Player.GetComponent<Target>(), _kickback);

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
