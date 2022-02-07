using UnityEngine;

namespace UltimateSurvival
{
    public class FPBowAnimator : FPAnimator
    {

        #region Data
#pragma warning disable 0649

        [Header("Bow")]

        [SerializeField] private float m_ReleaseSpeed = 1f;

        [SerializeField] private WeaponShake m_ReleaseShake;

#pragma warning restore 0649
        #endregion

        private FPBow m_Bow;


        protected override void Awake()
        {
            base.Awake();

            Player.Aim.AddStartListener(OnStart_Aim);
            Player.Aim.AddStopListener(OnStop_Aim);

            if (FPObject is FPBow)
            {
                m_Bow = (FPBow)FPObject;

                m_Bow.Attack.AddListener(On_Release);
                Animator.SetFloat("Release Speed", m_ReleaseSpeed);
            }
            else
                Debug.LogError("The animator is of type Bow, but no Bow script found on this game object!", this);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (FPObject && FPObject.IsEnabled && Animator)
                Animator.SetFloat("Release Speed", m_ReleaseSpeed);
        }

        private void OnStart_Aim()
        {
            if (FPObject.IsEnabled)
                Animator.SetBool("Aim", true);
        }

        private void OnStop_Aim()
        {
            if (FPObject.IsEnabled)
                Animator.SetBool("Aim", false);
        }

        private void On_Release()
        {
            Animator.SetBool("Aim", false);
            Animator.SetTrigger("Release");

            m_ReleaseShake.Shake();
        }
    }
}
