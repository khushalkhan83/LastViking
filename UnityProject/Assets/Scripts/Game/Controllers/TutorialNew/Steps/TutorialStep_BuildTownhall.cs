using Game.Models;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_BuildTownhall : TutorialStep_Building
    {
        public TutorialStep_BuildTownhall(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }
        public override void OnStart()
        {
            var houseBuildingType = HouseBuildingType.TownHall;
            
            houseBuilding = VillageBuildingModel.GetHouseBuildingByType(houseBuildingType);
            var houseInfo = VillageBuildingModel.GetHouseBuildingInfo(houseBuildingType);
            message = LocalizationModel.GetString(LocalizationKeyID.Tutorial_Build_Townhall);
            icon = houseInfo.icon;

            base.OnStart();
        }
    }
}