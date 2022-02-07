using Core.Providers;
using Game.Providers;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_variable_provider_controllerStates", menuName = "Variables/Providers/ControllerStates", order = 0)]
public class ControllerStatesProviderVariable : ScriptableObject
{
    [SerializeField] private  ControllerStatesProviderOld defaultValue;
    [SerializeField] private  ControllerStatesProviderOld debugValue;


    public ControllerStatesProviderOld Value
    {
        get
        {
            bool debugSettings = EditorGameSettings.Instance.debugControllersSettings;

            return debugSettings ? debugValue : defaultValue;
        }
    }

}