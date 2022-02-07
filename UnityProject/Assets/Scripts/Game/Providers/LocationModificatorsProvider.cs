using Core.Providers;
using Game.Controllers.Controllers.States.Modificators;
using Game.Models;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_modificators_location_default", menuName = "Providers/Modificators/Locations", order = 0)]
    public class LocationModificatorsProvider : ProviderScriptable<EnvironmentSceneID, LocationModificator>
    {
        [SerializeField] private LocationModificator _defaultLocationModificator;
        [EnumNamedArray(typeof(EnvironmentSceneID))]
        [SerializeField] private LocationModificator[] _data;
        public override LocationModificator this[EnvironmentSceneID key]
        {
            get
            {
                try
                {
                    return _data[((int)(object)key - 1)];
                }
                catch (System.Exception)
                {
                    Debug.Log($"{key.ToString()} not found. Default key value is used");
                    return _defaultLocationModificator;
                }
            }
        }

        public LocationModificator GetNullValue() => new LocationModificator();
    }
}
