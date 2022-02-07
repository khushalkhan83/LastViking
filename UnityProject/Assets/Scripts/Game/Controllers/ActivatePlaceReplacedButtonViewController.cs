using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class ActivatePlaceReplacedButtonViewController : ViewEnableController<PlaceButtonView>, IController
    {
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; private set; }
        [Inject] public SpritesProvider SpritesProvider { get; set; }

        public override ViewConfigID ViewConfigID => ViewConfigID.PlaceReplacedButtonConfig;
        public override IDataViewController Data => _data;
        public override bool IsCanShow => ReplaceBuildingModel.BuildingReplaceActive || ReplaceBuildingModel.ConstructionReplaceActive;

        private IDataViewController _data;

        public override void Enable()
        {
            _data = new PlaceButtonData(
                SpritesProvider[SpriteID.PlaceButtonActive], 
                SpritesProvider[SpriteID.PlaceButtonDefault]);

            ReplaceBuildingModel.OnReplaceBuildingActiveChanged += UpdateViewVisible;
            ReplaceBuildingModel.OnReplaceConstructionActiveChanged += UpdateViewVisible;
            UpdateViewVisible();
        }

        public override void Start()
        {
            
        }

        public override void Disable()
        {
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged -= UpdateViewVisible;
            ReplaceBuildingModel.OnReplaceConstructionActiveChanged -= UpdateViewVisible;
        }

    }
}
