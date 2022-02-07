using Game.Audio;
using System;
using System.Collections;
using UnityEngine;

public class EnvironmentOnceAudioThread : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private float _durationFade;
    [SerializeField] private Transform _containerAudio;

#pragma warning restore 0649
    #endregion

    private float DurationFade => _durationFade;
    private AudioSystem AudioSystem => AudioSystem.Instance;
    private Transform ContainerAudio => _containerAudio;

    private AudioObject Audio { get; set; }
    private Coroutine FadeShowProcess { get;  set; }

    private bool IsStopProcess { get; set; }
    public bool IsHasAudio => Audio != null;

    public void PlayOnce(AudioID audioID)
    {
        if (Audio != null && !IsStopProcess)
        {
            if (FadeShowProcess != null)
            {
                StopCoroutine(FadeShowProcess);
            }

            StartCoroutine(FadeTo(Audio, DurationFade, Audio.AudioSource.volume, 0, audio => AudioSystem.Release(audio)));
        }

        Audio = CreateAudio(audioID);
        FadeShowProcess = StartCoroutine(FadeTo(Audio, DurationFade, 0, Audio.AudioSource.volume, audio => FadeShowProcess = null));
    }

    public void StopAudio()
    {
        if (Audio != null)
        {
            IsStopProcess = true;
            StartCoroutine(FadeTo(Audio, DurationFade, Audio.AudioSource.volume, 0,
                audio =>
                {
                    AudioSystem.Release(audio);
                    Audio = null;
                    IsStopProcess = false;
                }
            ));
        }
    }

    private AudioObject CreateAudio(AudioID audioID)
    {
        var audio = AudioSystem.CreateAudio(audioID);
        audio.AudioSource.Play();

        audio.AudioSource.transform.SetParent(ContainerAudio);
        audio.AudioSource.transform.localPosition = Vector3.zero;

        return audio;
    }

    public IEnumerator FadeTo(AudioObject audio, float duration, float from, float to, Action<AudioObject> callback)
    {
        var remainingTime = duration;

        while (remainingTime > 0)
        {
            audio.AudioSource.volume = Mathf.Lerp(to, from, remainingTime / duration);
            yield return null;
            remainingTime -= Time.deltaTime;
        }

        audio.AudioSource.volume = to;

        callback?.Invoke(audio);
    }
}
