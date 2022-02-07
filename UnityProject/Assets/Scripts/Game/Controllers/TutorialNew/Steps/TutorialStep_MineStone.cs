using Game.Models;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_MineStone : TutorialStep_MineResource
    {
        public TutorialStep_MineStone(TutorialEvent StepStartedEvent,
                                      string requiaredResource,
                                      int resourceCount,
                                      int tokenConfigId,
                                      string tockenID,
                                      OutLineMinableObjectID outLineMinableObjectID,
                                      Sprite messageIcon,
                                      string notificationMessage) : base(StepStartedEvent, requiaredResource, resourceCount, tokenConfigId, tockenID, outLineMinableObjectID, messageIcon, notificationMessage)
        {
        }
    }
}