using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    public class AudioSystem : MonoBehaviour
    {
        private static AudioSystem instance;
        public static AudioSystem Instance
        {
            get
            {
                if(instance == null)
                {
                    var variable = Resources.Load<AudioSystemVariable>("SO_System_Audio");
                    instance = variable.Value;
                }
                return instance;
            }
        }

        #region Data
#pragma warning disable 0649
        [SerializeField] private AudioSystemVariable variable;
        [SerializeField] private AudiosProvider _audiosProider;
        [SerializeField] private AudioSourcesProvider _audioSourcesProvider;
        [SerializeField] private Transform _containerPool;
        [SerializeField] private Transform _containerPlay;
        [SerializeField] private AudioConfigurationMapper _audioConfigurationMapper;

#pragma warning restore 0649
        #endregion

        public AudiosProvider AudiosProvider => _audiosProider;
        public AudioSourcesProvider AudioSourcesProvider => _audioSourcesProvider;
        public AudioConfigurationMapper AudioConfigurationMapper => _audioConfigurationMapper;
        public Transform ContainerPool => _containerPool;
        public Transform ContainerPlay => _containerPlay;

        protected Dictionary<AudioSourceID, Stack<AudioSource>> Sources { get; } = new Dictionary<AudioSourceID, Stack<AudioSource>>();
        protected Stack<AudioObject> Audios { get; } = new Stack<AudioObject>();

        // Unity Engine
        private void Awake()
        {
            variable.Value = this;
        }

        public void PlayOnce(AudioID audioID)
        {
            StartCoroutine(PlayAudioOnceProcess(audioID));
        }

        public void PlayOnce(AudioID audioID, Vector3 position)
        {
            StartCoroutine(PlayAudioOnceProcess(audioID, position));
        }

        protected IEnumerator PlayAudioOnceProcess(AudioID audioID, Vector3 position)
        {
            var audio = CreateAudio(audioID);
            audio.AudioSource.Play();
            audio.AudioID = audioID;
            audio.AudioSource.transform.position = position;
            yield return new WaitForSeconds(audio.AudioSource.clip.length);
            Release(audio);
        }

        protected IEnumerator PlayAudioOnceProcess(AudioID audioID)
        {
            var audio = CreateAudio(audioID);
            audio.AudioSource.Play();
            audio.AudioID = audioID;
            yield return new WaitForSeconds(audio.AudioSource.clip.length);
            Release(audio);
        }

        public AudioObject CreateAudio(AudioID audioID)
        {
            var configuration = AudioConfigurationMapper[audioID];
            var sourceRef = AudioSourcesProvider[configuration.AudioSourceID];
            var audioRef = AudiosProvider[configuration.AudioResourceID];
            var audioSourceInstance = GetAudioSource(configuration.AudioSourceID, sourceRef);
            var audio = GetAudio();

            audio.AudioSource = audioSourceInstance;
            audio.AudioID = audioID;

            audioSourceInstance.clip = audioRef;
            audioSourceInstance.transform.SetParent(ContainerPlay);

            return audio;
        }

        public void Release(AudioObject audio)
        {
            var configuration = AudioConfigurationMapper[audio.AudioID];
            var audioSource = audio.AudioSource;
            var audioSourceID = configuration.AudioSourceID;
            var poolAudioSource = GetAudioSourcePool(audioSourceID);

            poolAudioSource.Push(audioSource);

            audio.AudioSource.transform.SetParent(ContainerPool);
            audio.AudioSource = null;
            audio.AudioID = AudioID.None;

            Audios.Push(audio);
        }

        private Stack<AudioSource> GetAudioSourcePool(AudioSourceID audioSourceID)
        {
            if (!Sources.ContainsKey(audioSourceID))
            {
                var poolAudioSource = new Stack<AudioSource>();
                Sources[audioSourceID] = poolAudioSource;

                return poolAudioSource;
            }

            return Sources[audioSourceID];
        }

        protected AudioObject GetAudio()
        {
            if (Audios.Count > 0)
            {
                return Audios.Pop();
            }

            return new AudioObject();
        }

        protected AudioSource GetAudioSource(AudioSourceID audioSourceID, AudioSource @ref)
        {
            if (Sources.ContainsKey(audioSourceID))
            {
                var pool = Sources[audioSourceID];
                if (pool.Count > 0)
                {
                    return CopyAudioSourceFrom(pool.Pop(), @ref);
                }
            }

            return Instantiate(@ref);
        }

        protected AudioSource CopyAudioSourceFrom(AudioSource to, AudioSource from)
        {
            to.gameObject.SetActive(from.gameObject.activeSelf);
            to.transform.position = from.transform.position;
            to.transform.rotation = from.transform.rotation;
            to.transform.localScale = from.transform.localScale;
            to.clip = from.clip;
            to.bypassEffects = from.bypassEffects;
            to.bypassListenerEffects = from.bypassListenerEffects;
            to.bypassReverbZones = from.bypassReverbZones;
            to.dopplerLevel = from.dopplerLevel;
            to.enabled = from.enabled;
            to.hideFlags = from.hideFlags;
            to.ignoreListenerPause = from.ignoreListenerPause;
            to.ignoreListenerVolume = from.ignoreListenerVolume;
            to.loop = from.loop;
            to.maxDistance = from.maxDistance;
            to.minDistance = from.minDistance;
            to.mute = from.mute;
            to.name = from.name;
            to.outputAudioMixerGroup = from.outputAudioMixerGroup;
            to.panStereo = from.panStereo;
            to.pitch = from.pitch;
            to.playOnAwake = from.playOnAwake;
            to.priority = from.priority;
            to.reverbZoneMix = from.reverbZoneMix;
            to.rolloffMode = from.rolloffMode;
            to.spatialBlend = from.spatialBlend;
            to.spatialize = from.spatialize;
            to.spatializePostEffects = from.spatializePostEffects;
            to.spread = from.spread;
            to.tag = from.tag;
            to.time = from.time;
            to.timeSamples = from.timeSamples;
            to.velocityUpdateMode = from.velocityUpdateMode;
            to.volume = from.volume;

            return to;
        }
    }
}
