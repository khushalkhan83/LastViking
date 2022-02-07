
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Game.Objectives;

namespace Game.Controllers.Controllers.States.Modificators
{
    public enum ProcessingType {Activate, Deactivate}

    public abstract class ModificatorBase : ScriptableObject, IState
    {
        [SerializeField] private ControllerCollectionProcessingTypeDictionary modificators = new ControllerCollectionProcessingTypeDictionary();

        public event Action<ControllerID> OnActivateController;
        public event Action<ControllerID> OnDeactivateController;
        public event Action<ControllerID> OnStartController;

        void IState.Enter() { ActivateDeactivateControllers(true); TryStartControllers(true);}
        void IState.Exit() { ActivateDeactivateControllers(false); TryStartControllers(false);}

        public bool CanActivate(ControllerID controller) => CanUseController(controller,ProcessingType.Activate);
        public bool CanDeactivate(ControllerID controller) => CanUseController(controller,ProcessingType.Deactivate);


        bool wroteMessage = false;

        private void ActivateDeactivateControllers(bool activate)
        {
            wroteMessage = false;

            foreach (var collection in modificators.Keys)
            {
                var ids = collection.ControllerIDs;
                if(ids.Count() == 0) continue;

                var processingType = modificators[collection];

                foreach (var id in ids)
                {
                    if(wroteMessage == false) LogMessage(activate);
                    var strategy = GetProcecingStrategy(processingType,activate);
                    strategy?.Invoke(id);
                }
            }
        }

        private void TryStartControllers(bool activate)
        {
            foreach (var collection in modificators.Keys)
            {
                var ids = collection.ControllerIDs;
                if (ids.Count() == 0) continue;

                var processingType = modificators[collection];

                if(processingType != ProcessingType.Activate) continue;

                foreach (var id in ids)
                {
                    OnStartController?.Invoke(id);
                }
            }
        }

        private void LogMessage(bool activate)
        {
            // if (activate)
            //     Debug.Log($"<color=red>Apply Modificator: {this.name} </color>");
            // else
            //     Debug.Log($"<color=red>Remove Modificator: {this.name} </color>");

            wroteMessage = true;
        }

        private bool CanUseController(ControllerID controller, ProcessingType processingType)
        {
            bool result = false;
            foreach (var collection in modificators.Keys)
            {
                var ids = collection.ControllerIDs;
                var procType = modificators[collection];

                if(procType == processingType)
                {
                    result = ids.Contains(controller);
                }

                if(result == true) break;
            }
            return result;
        }

        private Action<ControllerID> GetProcecingStrategy(ProcessingType processingType, bool activate)
        {
            switch(processingType)
            {
                case ProcessingType.Activate:
                    return activate ? OnActivateController : OnDeactivateController;
                case ProcessingType.Deactivate:
                    return activate ? OnDeactivateController : OnActivateController;
                default:
                    throw new Exception();
            }
        }



        // #region Editor related
        // TODO: on validate each frame. Remove or redo
        // private void OnValidate() {
        //     #if !UNITY_EDITOR
        //     return;
        //     #endif

        //     var controllersIds = modificators.Keys.SelectMany(x => x.ControllerIDs);
        //     var hash = new HashSet<ControllerID>();
        //     var duplicates = controllersIds.Where(i => !hash.Add(i));

        //     if(duplicates.Count() == 0) return;

        //     string dublicatesText = string.Empty;
        //     duplicates.ToList().ForEach(x => dublicatesText += x.ToString() + " ");

        //     EditorUtility.DisplayDialog("Внимание", $"Обнаруженны дубликаты контроллеров в колекциях. {dublicatesText}. Это поведение не поддерживаеться.", "ОК");
        // }

        // #endregion
    }
}
