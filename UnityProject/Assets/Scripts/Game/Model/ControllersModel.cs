using Game.Controllers;
using Game.Controllers.Controllers.States;
using Game.Controllers.Controllers.States.Modificators;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class ControllersModel : MonoBehaviour
    {
        public const ControllersStateID PreInitControllersStateID = ControllersStateID.PreInit;
        public const ControllersStateID StartControllersStateID = ControllersStateID.StartUp;

        public ControllersStateID ControllersStateID { get; private set; }
        public HashSet<ControllerID> ActiveControllerIDs { get; } = new HashSet<ControllerID>();
        public HashSet<ControllerID> StartedControllersIDs { get; } = new HashSet<ControllerID>();

        public event Action<ControllersStateID> OnApplyState;
        // public event Action<AddRemoveModificator> OnModificatorUsed;

        public event Action<ModificatorBase> OnApplyModificator;

        public event Action<ControllerIDCollectionBase> OnOldEnvironmentModificatorRemoved;
        public event Action<ControllerIDCollectionBase> OnNewEnvironmentModificatorAdded;

        public void ApplyState(ControllersStateID controllersStateID)
        {
            ControllersStateID = controllersStateID;
            OnApplyState?.Invoke(controllersStateID);
        }

       public void ApplyModificator(ModificatorBase stateModificator)
       {
           OnApplyModificator?.Invoke(stateModificator);
       }
    }
}
