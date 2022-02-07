using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(fileName = "SO_System_Audio", menuName = "Variables/Systems/Audio", order = 15)]
    public class AudioSystemVariable : ScriptableObject
    {
        public AudioSystem Value;
    }
}
