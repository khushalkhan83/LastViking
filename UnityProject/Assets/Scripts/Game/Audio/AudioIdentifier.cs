using Game.Audio;
using UnityEngine;

public class AudioIdentifier : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private AudioID[] _audioID;

#pragma warning restore 0649
    #endregion

    public AudioID[] AudioID => _audioID;
}
