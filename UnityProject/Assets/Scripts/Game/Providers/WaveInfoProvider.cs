using Core.Providers;
using Game.Models;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_waveInfo_default", menuName = "Providers/WafeInfo", order = 0)]

    public class WaveInfoProvider : ProviderScriptable<WaveInfoId, WaveInfoData>
    {
        [EnumNamedArray(typeof(WaveInfoId))]
        [SerializeField] private WaveInfoData[] _data;

        public override WaveInfoData this[WaveInfoId key] => _data[((int)(object)key - 1)];
    }
}
