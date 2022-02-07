using UnityEngine;
using ActionsCollections;

namespace DebugActions
{
    public class TeleportActionToWaterfall : ActionBase
    {
        public override string OperationName => "ToFolder";

        public override void DoAction()
        {
            Debug.Log("Teleport");
        }
    }
}
