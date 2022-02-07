using Core;
using Core.Controllers;
using Game.Models;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Controllers
{
    public class DebugInputController : IDebugInputController, IController
    {
        [Inject] public EncountersViewModel EncountersViewModel { get; private set; }

        private EncountersModel EncountersModel {get;} = GameObject.FindObjectOfType<EncountersModel>();

        private static DebugInputController instance;
        
        void IController.Enable() 
        {
            instance = this;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
        }


        #if UNITY_EDITOR
        [MenuItem("EditorTools/DebugInput/Encounters _F3")] // F3 to use
        private static void _SwitchEncountersView() => instance?.SwitchEncountersView();

        [MenuItem("EditorTools/DebugInput/ResetEncounters _F4")] // F4 to use
        private static void _ResetEncounters() => instance?.ResetEncounters();
        #endif

        private void SwitchEncountersView() => EncountersViewModel.Switch();
        private void ResetEncounters() => EncountersModel.Reset();
    }
}
