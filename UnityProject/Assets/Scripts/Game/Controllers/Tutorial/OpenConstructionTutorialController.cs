using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class OpenConstructionTutorialController : ViewEnableController<ConstructionInfoPopupView>, IOpenConstructionTutorialController
    {
        public override ViewConfigID ViewConfigID => ViewConfigID.OpenConstructionButtonConfig;
        [Inject] public OpenConstructionTutorialModel OpenConstructionTutorialModel{get;set;}
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }


        public override bool IsCanShow => true;


        public override void Enable()
        {
            BuildingModeModel.HideSwitchButton = true;
            OpenConstructionTutorialModel.OnPlayerEnterConstructionZone += OnPlayerPlayerEnterConstructionZone;
        }
        
        public override void Disable()
        {
            BuildingModeModel.HideSwitchButton = false;
            OpenConstructionTutorialModel.OnPlayerEnterConstructionZone -= OnPlayerPlayerEnterConstructionZone;
            Hide();
        }

        public override void Start()
        {
           
        }

        private void OnPlayerPlayerEnterConstructionZone()
        {
            UpdateViewVisible();
        }


    }
}
