using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class TombCameraController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _speedMove;
        [SerializeField] private float _speedSmooth;
        [SerializeField] private float _distanceMove;
        [SerializeField] private float _durationGrayscale;
        [SerializeField] private GrayscalePostEffect _grayscalePostEffect;

#pragma warning restore 0649
        #endregion

        public float SpeedMove => _speedMove;
        public float SpeedSmooth => _speedSmooth;
        public float DurationGrayscale => _durationGrayscale;
        public float DistanceMove => _distanceMove;
        public GrayscalePostEffect GrayscalePostEffect => _grayscalePostEffect;

        public Vector3 TargetPosition { get; private set; }
        public float Delta { get; private set; }

        protected Coroutine FadeGrayscale { get; private set; }
        protected Vector3 StartPosition { get; private set; }
        protected Vector3 SpeedDirection { get; private set; }

        private void Start()
        {
            TargetPosition = transform.position;
            StartPosition = transform.position;
            SpeedDirection = -transform.forward;
        }

        private void OnEnable()
        {
            BeginFadeGrayscaleProcess();
        }

        private void OnDisable()
        {
            EndFadeGrayscaleProcess();
        }

        private void Update()
        {
            transform.position = TargetPosition;

            TargetPosition = transform.position + SpeedDirection * SpeedMove * Delta;
            Delta = Mathf.Lerp(Delta, Time.unscaledDeltaTime, SpeedSmooth);

            var dis = Vector3.Distance(transform.position, StartPosition);

            if (dis > DistanceMove)
            {
                SpeedDirection = transform.forward;
            }
            else if (Vector3.Distance(transform.position, StartPosition + transform.forward) - dis < 0f)
            {
                SpeedDirection = -transform.forward;
            }
        }

        public void BeginFadeGrayscaleProcess()
        {
            FadeGrayscale = StartCoroutine(FadeGrayscaleProcess());
        }

        public void EndFadeGrayscaleProcess()
        {
            if (FadeGrayscale != null)
            {
                StopCoroutine(FadeGrayscale);
                FadeGrayscale = null;
            }
        }

        public IEnumerator FadeGrayscaleProcess()
        {
            GrayscalePostEffect.SetBlend(0);
            yield return null;
            yield return null;

            var duration = DurationGrayscale;
            do
            {
                GrayscalePostEffect.SetBlend(1 - duration / DurationGrayscale);
                yield return null;
                duration -= Time.unscaledDeltaTime;

            } while (duration >= 0);
            GrayscalePostEffect.SetBlend(1);
        }
    }
}
