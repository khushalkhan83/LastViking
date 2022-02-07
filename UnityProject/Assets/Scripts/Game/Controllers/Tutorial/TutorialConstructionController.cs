using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TutorialConstructionController : ViewEnableController<ConstructionTutorialView>, ITutorialConstructionController
    {
        public override ViewConfigID ViewConfigID => ViewConfigID.ConstructionTutorialConfig;
        [Inject] public TutorialHouseModel TutorialHouseModel{get;set;}
        [Inject] public BuildingModeModel BuildingModeModel{get;set;}

        public override bool IsCanShow => canShow;

        private bool canShow;


        public override void Enable()
        {
            TutorialHouseModel.OnPlayerEnterConstructionZone += OnPlayerPlayerEnterConstructionZone;
        }
        
        public override void Disable()
        {
            BuildingModeModel.BuildingActive = false;
            TutorialHouseModel.OnPlayerEnterConstructionZone -= OnPlayerPlayerEnterConstructionZone;
            Hide();
        }

        public override void Start()
        {
           
        }

        private void OnPlayerPlayerEnterConstructionZone()
        {
            canShow = true;
            UpdateViewVisible();
        }
    }
}
