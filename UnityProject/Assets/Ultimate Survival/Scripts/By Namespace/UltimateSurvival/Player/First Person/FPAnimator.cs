using UnityEngine;

namespace UltimateSurvival
{
    [RequireComponent(typeof(FPObject))]
    public class FPAnimator : PlayerBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator m_Animator;
        [SerializeField] private float m_DrawSpeed = 1f;
        [SerializeField] private float m_HolsterSpeed = 1f;

#pragma warning restore 0649
        #endregion

        private bool m_Initialized;

        public Animator Animator => m_Animator;
        public float DrawSpeed => m_DrawSpeed;
        public float HolsterSpeed => m_HolsterSpeed;

        public FPObject FPObject { get; private set; }

        protected virtual void Awake()
        {
            FPObject = GetComponent<FPObject>();

            FPObject.Draw.AddListener(On_Draw);
            FPObject.Holster.AddListener(On_Holster);
            Player.Respawn.AddListener(On_Respawn);

            Animator.SetFloat("Draw Speed", DrawSpeed);
            Animator.SetFloat("Holster Speed", HolsterSpeed);
        }

        protected virtual void OnValidate()
        {
            if (FPObject && FPObject.IsEnabled && Animator)
            {
                Animator.SetFloat("Draw Speed", DrawSpeed);
                Animator.SetFloat("Holster Speed", HolsterSpeed);
            }
        }

        private void On_Draw()
        {
            OnValidate();

            if (Animator)
                Animator.SetTrigger("Draw");
        }

        private void On_Holster()
        {
            if (Animator)
                Animator.SetTrigger("Holster");
        }

        private void OnStop_Sleep()
        {
            if (FPObject.IsEnabled)
                OnValidate();
        }

        private void On_Respawn()
        {
            if (FPObject.IsEnabled)
                OnValidate();
        }
    }
}
