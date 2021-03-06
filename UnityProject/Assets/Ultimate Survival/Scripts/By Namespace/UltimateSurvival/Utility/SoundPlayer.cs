using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
    [Serializable]
    public class SoundPlayer
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AudioClip[] m_Sounds;

        [SerializeField] private Vector2 m_VolumeRange = new Vector2(0.5f, 0.75f);

        [SerializeField] private Vector2 m_PitchRange = new Vector2(0.9f, 1.1f);

#pragma warning restore 0649
        #endregion

        private int m_LastSoundPlayed;

        public void Play(ItemSelectionMethod selectionMethod, AudioSource audioSource, float volumeFactor = 1f)
        {
            if (!audioSource || m_Sounds.Length == 0)
                return;

            int clipToPlay = CalculateNextClipToPlay(selectionMethod);

            var volume = Random.Range(m_VolumeRange.x, m_VolumeRange.y) * volumeFactor;
            audioSource.pitch = Random.Range(m_PitchRange.x, m_PitchRange.y);

            audioSource.PlayOneShot(m_Sounds[clipToPlay], volume);

            m_LastSoundPlayed = clipToPlay;
        }

        /// <summary>
        /// Will use the AudioSource.PlayClipAtPoint() method, which doesn't include pitch variation.
        /// </summary>
        public void PlayAtPosition(ItemSelectionMethod selectionMethod, Vector3 position, float volumeFactor = 1f)
        {
            if (m_Sounds.Length == 0)
                return;

            int clipToPlay = CalculateNextClipToPlay(selectionMethod);

            AudioSource.PlayClipAtPoint(m_Sounds[clipToPlay], position, Random.Range(m_VolumeRange.x, m_VolumeRange.y) * volumeFactor);

            m_LastSoundPlayed = clipToPlay;
        }

        public void Play2D(ItemSelectionMethod selectionMethod = ItemSelectionMethod.RandomlyButExcludeLast)
        {
            if (m_Sounds.Length == 0)
                return;

            int clipToPlay = CalculateNextClipToPlay(selectionMethod);
            GameController.Audio.Play2D(m_Sounds[clipToPlay], Random.Range(m_VolumeRange.x, m_VolumeRange.y));
        }

        private int CalculateNextClipToPlay(ItemSelectionMethod selectionMethod)
        {
            int clipToPlay = 0;

            if (selectionMethod == ItemSelectionMethod.Randomly || m_Sounds.Length == 1)
                clipToPlay = Random.Range(0, m_Sounds.Length);
            else if (selectionMethod == ItemSelectionMethod.RandomlyButExcludeLast)
            {
                // Place the last played sound first in the array.
                AudioClip firstClip = m_Sounds[0];
                m_Sounds[0] = m_Sounds[m_LastSoundPlayed];
                m_Sounds[m_LastSoundPlayed] = firstClip;

                // Then play a random sound but exclude the first one.
                clipToPlay = Random.Range(1, m_Sounds.Length);
            }
            else if (selectionMethod == ItemSelectionMethod.InSequence)
                clipToPlay = (int)Mathf.Repeat(m_LastSoundPlayed + 1, m_Sounds.Length);

            return clipToPlay;
        }
    }
}