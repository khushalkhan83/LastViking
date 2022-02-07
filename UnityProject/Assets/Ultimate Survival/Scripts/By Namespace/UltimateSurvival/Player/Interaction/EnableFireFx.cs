using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class EnableFireFx : MonoBehaviour
    {

        public ChangedValue<bool> IsBurning = new ChangedValue<bool>(false);

        #region Data
#pragma warning disable 0649

        [Header("Fire")]

        [SerializeField] private ParticleSystem m_Fire;

        [SerializeField] private AudioSource m_FireSource;

#pragma warning restore 0649
        #endregion

        private void Start()
        {
            IsBurning.SetForce(false);
        }

        private void OnEnable()
        {
            IsBurning.OnChange += OnChanged_IsBurning;
        }

        private void OnDisable()
        {
            IsBurning.OnChange -= OnChanged_IsBurning;
        }

        private void OnChanged_IsBurning()
        {
            if (Building.PlayerBuilder.close)
            {
                m_Fire.Play(true);
                GameController.Audio.LerpVolumeOverTime(m_FireSource, 0.5f, 3f);
            }
            else
            {
                m_Fire.Stop(true);
                GameController.Audio.LerpVolumeOverTime(m_FireSource, 0f, 3f);
            }
        }
    }
}
