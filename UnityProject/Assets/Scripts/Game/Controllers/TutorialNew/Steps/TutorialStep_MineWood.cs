using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_MineWood : TutorialStep_MineResource
    {
        private const string k_mineToolID = "tool_hook";

        public TutorialStep_MineWood(TutorialEvent StepStartedEvent, 
                                     string requiaredResource,
                                     int resourceCount,
                                     int tokenConfigId,
                                     string tockenID,
                                     OutLineMinableObjectID outLineMinableObjectID,
                                     Sprite messageIcon,
                                     string notificationMessage) : base(StepStartedEvent, requiaredResource, resourceCount, tokenConfigId, tockenID, outLineMinableObjectID, messageIcon, notificationMessage)
        {
        }

        public override void OnStart()
        {
            InventoryOperationsModel.TryEquipItem(k_mineToolID);
            base.OnStart();
        }
    }
}