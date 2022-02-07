using System.Linq;
using UnityEngine;
using RoboRyanTron.SearchableEnum;

namespace Game.Controllers.Controllers.States
{

    [CreateAssetMenu(fileName = "SO_CCollection_debug_new", menuName = "Controllers/Collections/DebugCollection", order = 0)]
    public class DebugControllerIDCollection : ControllerIDCollectionBase
    {
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        private bool debugMode => EditorGameSettings.debugControllersSettings;

        [SearchableEnum]
        [SerializeField] private ControllerID[] regularControllerIDs;
        [SearchableEnum]
        [SerializeField] private ControllerID[] debugModeControllerIDs;

        public override ControllerID[] ControllerIDs => debugMode ? debugModeControllerIDs: regularControllerIDs;

    }
}
