using Core.Providers;
using Game.Controllers;
using Game.Controllers.Controllers.States;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_controllerStates_new", menuName = "Providers/ControllerStates", order = 0)]
    public class ControllerStatesProvider : ProviderScriptable<ControllersStateID, IControllersState>
    {
        [EnumNamedArray(typeof(ControllersStateID))]

        [SerializeField] private ControllersState[] _data;

        public override IControllersState this[ControllersStateID key] => _data[((int)(object)key - 1)];
    }
}
