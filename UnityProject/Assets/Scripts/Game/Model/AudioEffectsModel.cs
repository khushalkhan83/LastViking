using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Models
{
    public class AudioEffectsModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private float slowMotionPitch;
        [SerializeField] private string slowMotionParamName;

        #pragma warning restore 0649
        #endregion


        public event Action<bool> OnSlowMotion;
        public void SlowMotion(bool activate)
        {
            audioMixer.SetFloat(slowMotionParamName,activate ? slowMotionPitch : 1);
            OnSlowMotion?.Invoke(activate);
        }

        public event Action OnRemoeAllEffects;
        
        public void RemoeAllEffects()
        {
            OnRemoeAllEffects?.Invoke();
        }
    }
}
