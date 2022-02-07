using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

public class ShelterCursorUpgradeViewController : ViewControllerBase<ShelterCursorUpgradeView>
{
    [Inject] public ViewsSystem ViewsSystem { get; protected set; }
    [Inject] public ShelterUpgradeModel ShelterUpgradeModel { get; protected set; }
    [Inject] public InputModel InputModel { get; protected set; }

    protected override void Show()
    {
        View.OnInteract += OnDownPointHandler;
    }

    protected override void Hide()
    {
        View.OnInteract -= OnDownPointHandler;
    }

    private void OnDownPointHandler()
    {
        ShelterUpgradeModel.InteractWithShelterUpgradeTable();
    }
}
