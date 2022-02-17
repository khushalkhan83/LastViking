using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_BuildTools : TutorialStep_Building
    {
        public TutorialStep_BuildTools(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        public override void OnStart()
        {
            houseBuilding = ModelsSystem.Instance._villageBuildingModel.GetHouseBuildingByType(Models.HouseBuildingType.Tools);
            message = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Build_Tools);
            icon = ModelsSystem.Instance._villageBuildingModel.GetHouseBuildingInfo(Models.HouseBuildingType.Tools).icon;
            base.OnStart();
        }
    }
}
