using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.Views;
using UltimateSurvival;

public class ShelterCursorActivateViewController : ViewControllerBase<ShelterCursorActivateView>
{
    [Inject] public ShelterModelsProvider ShelterModelsProvider { get; private set; }
    [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
    [Inject] public ViewsSystem ViewsSystem { get; private set; }

    protected ShelterModel ShelterModel { get; private set; }

    protected override void Show()
    {
        ShelterModel = ShelterModelsProvider[PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<TableUpgradeShelterView>().ShelterModelID];
        View.OnDownPoint += OnDownPointHandler;
    }

    protected override void Hide()
    {
        View.OnDownPoint -= OnDownPointHandler;
    }

    private void OnDownPointHandler() => ShelterModel.Activate();
}
