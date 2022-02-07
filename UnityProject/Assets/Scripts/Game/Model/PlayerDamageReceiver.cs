using Game.Audio;
using Game.Views;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Models
{
    public class PlayerDamageReceiver : MonoBehaviour, IDamageable
    {
        #region Data
#pragma warning disable 0649

#pragma warning restore 0649
        #endregion

        public event Action<float, GameObject> OnTakeDamage;

        private AudioSystem AudioSystem => AudioSystem.Instance;

        private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;

        private IHealth Health => ModelsSystem.Instance._playerHealthModel;

        private AudioIdentifier _audioIdentifier;
        private AudioIdentifier AudioIdentifier => _audioIdentifier ?? (_audioIdentifier = GetComponent<AudioIdentifier>());

        public ReceiveDamageView ReceiveDamageView { get; private set; }

        public void Damage(float value, GameObject from = null)
        {
            if (!PlayerDeathModel.IsImmunable)
            {
                if (AudioIdentifier)
                {
                    AudioSystem.PlayOnce(AudioIdentifier.AudioID[UnityEngine.Random.Range(0, AudioIdentifier.AudioID.Length)]);
                }
                Health.AdjustHealth(-value);
                OnTakeDamage?.Invoke(value, from);
                StartCoroutine(ShowFader());
            }
        }

        private IEnumerator ShowFader(Action callback = null)
        {
            if (ReceiveDamageView != null)
            {
                ViewsSystem.Hide(ReceiveDamageView);
            }

            ReceiveDamageView = ViewsSystem.Show<ReceiveDamageView>(ViewConfigID.ReceiveDamage);
            ReceiveDamageView.StopAllCoroutines();

            yield return ReceiveDamageView.Fade(ReceiveDamageView.ColorFrom, ReceiveDamageView.ColorTo, ReceiveDamageView.DurationIn);
            yield return ReceiveDamageView.Fade(ReceiveDamageView.ColorTo, ReceiveDamageView.ColorFrom, ReceiveDamageView.DurationOut);

            ViewsSystem.Hide(ReceiveDamageView);
            callback?.Invoke();
        }
    }
}
