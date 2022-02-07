using System.Linq;
using UnityEngine;
using RoboRyanTron.SearchableEnum;

namespace Game.Controllers.Controllers.States
{
    public abstract class ControllerIDCollectionBase : ScriptableObject
    {
        public abstract ControllerID[] ControllerIDs {get;}

    }
}
