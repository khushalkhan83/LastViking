using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Game.Controllers
{
    namespace Controllers.States
    {
        [CreateAssetMenu(fileName = "SO_controllersState_new", menuName = "Controllers/States/ControllersState", order = 0)]
        public class ControllersState: ScriptableObject, IControllersState
        {
            [System.Serializable]
            public class Data
            {
                [SerializeField] private List<ControllerIDCollectionBase> controllers = new List<ControllerIDCollectionBase>();
                [SerializeField] private ControllerProcessingID controllerProcessingID;

                public ControllerID[] ControllerIDs => controllers.SelectMany(x => x.ControllerIDs).ToArray();
                public ControllerProcessingID ControllerProcessingID => controllerProcessingID;
            }

            [SerializeField] private Data enter;
            [SerializeField] private Data exit;

            public event Action<ControllerProcessingID, ControllerID[]> OnProcessState
            {
                add => onProcessModificator+= value;
                remove => onProcessModificator -= value;
            }
            event Action<ControllerProcessingID,ControllerID[]> onProcessModificator;


            public void Enter()
            {
                var ids = enter.ControllerIDs;
                var config = enter.ControllerProcessingID;

                onProcessModificator?.Invoke(config,ids.ToArray());
            }

            public void Exit()
            {
                var ids = exit.ControllerIDs;
                var config = exit.ControllerProcessingID;

                onProcessModificator?.Invoke(config,ids.ToArray());
            }
        }
    }
}
