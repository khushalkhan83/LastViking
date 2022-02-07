using System.Collections;
using UnityEngine;

namespace Game.Audio
{
    public class OnceAudioThread : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _durationFade;
        [SerializeField] private Transform _containerAudio;

#pragma warning restore 0649
        #endregion

        public float DurationFade => _durationFade; 
        public AudioSystem AudioSystem => AudioSystem.Instance;
        public Transform ContainerAudio => _containerAudio;

        public AudioObject Audio { get; private set; }
        public Coroutine AudioPlayOnceProcess { get; set; }

        public void PlayOnce(AudioID audioID)
        {
            if (Audio != null)
            {
                StopCoroutine(AudioPlayOnceProcess);
                StartCoroutine(FadeAudio(Audio, DurationFade));
            }

            Audio = AudioSystem.CreateAudio(audioID);
            AudioPlayOnceProcess = StartCoroutine(PlayAudioOnceProcess());
        }

        private IEnumerator FadeAudio(AudioObject audio, float durationFade)
        {
            var time = Mathf.Min(audio.AudioSource.clip.length - audio.AudioSource.time, durationFade);
            var remainingTime = time;
            var volume = audio.AudioSource.volume;

            while (remainingTime > 0)
            {
                yield return null;
                remainingTime -= Time.deltaTime;
                audio.AudioSource.volume = volume * (remainingTime / time);
            }
            AudioSystem.Release(audio);
        }

        private IEnumerator PlayAudioOnceProcess()
        {
            Audio.AudioSource.Play();
            Audio.AudioSource.transform.SetParent(ContainerAudio);
            Audio.AudioSource.transform.localPosition = Vector3.zero;
            yield return new WaitForSeconds(Audio.AudioSource.clip.length);
            AudioSystem.Release(Audio);
            Audio = null;
        }
    }
}
