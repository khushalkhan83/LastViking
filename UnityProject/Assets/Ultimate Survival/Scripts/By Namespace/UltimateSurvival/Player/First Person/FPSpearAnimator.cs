using UnityEngine;

namespace UltimateSurvival
{
    /// <summary>
    /// 
    /// </summary>
    public class FPSpearAnimator : FPAnimator
    {

        #region Data
#pragma warning disable 0649

        [Header("Spear")]

        [SerializeField] private float m_ThrowSpeed = 1f;

        [SerializeField] private WeaponShake m_ThrowShake;

#pragma warning restore 0649
        #endregion

        private FPSpear m_Spear;


        protected override void Awake()
        {
            base.Awake();

            Player.Aim.AddStartListener(OnStart_Aim);
            Player.Aim.AddStopListener(OnStop_Aim);

            if (FPObject as FPSpear)
            {
                m_Spear = FPObject as FPSpear;

                m_Spear.Attack.AddListener(On_Thrown);
                Animator.SetFloat("Throw Speed", m_ThrowSpeed);
            }
            else
                Debug.LogError("The animator is of type Spear, but no Spear script found on this game object!", this);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (FPObject && FPObject.IsEnabled && Animator)
                Animator.SetFloat("Throw Speed", m_ThrowSpeed);
        }

        private void OnStart_Aim()
        {
            if (FPObject.IsEnabled)
            {
                //Animator.Play("Hold Pose", 0, 0f);
                Animator.SetBool("Ready", true);
            }
        }

        private void OnStop_Aim()
        {
            if (FPObject.IsEnabled)
            {
                Animator.Play("Idle", 0, 0f);
                Animator.SetBool("Ready", false);
            }
        }

        private void On_Thrown()
        {
            Animator.SetBool("Ready", false);
            Animator.SetTrigger("Throw");
            m_ThrowShake.Shake();
        }
    }
}
