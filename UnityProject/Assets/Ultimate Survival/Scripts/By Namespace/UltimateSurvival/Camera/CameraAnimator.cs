using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    public class CameraAnimator : PlayerBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator m_Animator;

#pragma warning restore 0649
        #endregion

        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;

        private void OnEnable()
        {
            PlayerRunModel.OnChangeIsRun += OnChangeIsRunHandler;
        }

        private void OnDisable()
        {
            PlayerRunModel.OnChangeIsRun -= OnChangeIsRunHandler;
        }

        private void OnChangeIsRunHandler()
        {
            m_Animator.SetBool("Run", PlayerRunModel.IsRun);
        }
    }
}
