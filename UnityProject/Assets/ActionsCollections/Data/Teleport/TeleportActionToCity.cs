using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public class TeleportActionToCity : ActionBase
    {
        public override string OperationName => "ToIsland";

        public override void DoAction()
        {
            Debug.Log("To Island");
        }
    }
}