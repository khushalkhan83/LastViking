using Core.Providers;
using Game.Controllers;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_controllerStates_default", menuName = "Providers/ControllerStatesOld", order = 0)]

    public class ControllerStatesProviderOld : ProviderScriptable<ControllersStateID, ControllerStateConfigData>
    {
        [EnumNamedArray(typeof(ControllersStateID))]

        [SerializeField] private ControllerStateConfigData[] _data;

        public override ControllerStateConfigData this[ControllersStateID key] => _data[((int)(object)key - 1)];
    }
}
