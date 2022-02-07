using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class DiscordModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredString _url;
        [SerializeField] private ObscuredFloat _duration;
        [SerializeField] private ObscuredInt _deathCount;

#pragma warning restore 0649
        #endregion

        public float Duration => _duration;

        public string URL => _url;
        public int DeathCount => _deathCount;
    }
}
