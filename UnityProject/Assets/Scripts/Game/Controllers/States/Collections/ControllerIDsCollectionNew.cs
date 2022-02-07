using System.Linq;
using UnityEngine;
using RoboRyanTron.SearchableEnum;

namespace Game.Controllers.Controllers.States
{

    [CreateAssetMenu(fileName = "SO_CCollection_new", menuName = "Controllers/Collections/Collection", order = 0)]
    public class ControllerIDsCollectionNew : ControllerIDCollectionBase
    {
        [SearchableEnum]
        [SerializeField] private ControllerID[] controllerIDs;

        public override ControllerID[] ControllerIDs => controllerIDs;
    }
}
