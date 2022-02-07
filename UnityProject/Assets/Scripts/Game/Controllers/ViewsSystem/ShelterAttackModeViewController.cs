using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using Game.VillageBuilding;

public class ShelterAttackModeViewController : ViewControllerBase<ObjectiveAttackProcessView>
{
    [Inject] public GameTimeModel GameTimeModel { get; private set; }
    [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
    [Inject] public VillageBuildingModel VillageBuildingModel {get; private set;}
    [Inject] public VillageAttackModel VillageAttackModel {get; private set;}

    private HouseBuilding Townhall => VillageBuildingModel.Townhall;
    protected IHealth HealthCurrent { get; private set; }

    protected override void Show()
    {
        View.PlayShow();

        HealthCurrent = Townhall.GetComponentInChildren<IHealth>();
        HealthCurrent.OnChangeHealth -= OnChangeHealthHandler;
        HealthCurrent.OnChangeHealth += OnChangeHealthHandler;
        GameUpdateModel.OnUpdate += OnUpdate;

        UpdateShelterHealthAmount();
    }

    protected override void Hide()
    {
        View.PlayHide(); // don't work because view isn't ViewAnimationBase

        GameUpdateModel.OnUpdate -= OnUpdate;
    }

    private void OnChangeHealthHandler() => UpdateShelterHealthAmount();

    private void UpdateShelterHealthAmount()
    {
        View.SetFillAmount(HealthCurrent.Health / HealthCurrent.HealthMax);
        View.SetSliderValue(HealthCurrent.Health / HealthCurrent.HealthMax);
        View.SetShelterHPText($"{Math.Ceiling(HealthCurrent.Health)} / {HealthCurrent.HealthMax}");
    }

    private void OnUpdate()
    {
        View.SetZombiesAliveText($"{VillageAttackModel.EnemiesLeft} / {VillageAttackModel.EnemiesTotal}");
        if (GameTimeModel.GetHours(GameTimeModel.EnviroTimeOfDayTicks) < 24 && GameTimeModel.GetHours(GameTimeModel.EnviroTimeOfDayTicks) > 12)
        {
            View.SetTimeText($"{30 - GameTimeModel.GetHours(GameTimeModel.EnviroTimeOfDayTicks):00}:{60 - GameTimeModel.GetMinutes(GameTimeModel.EnviroTimeOfDayTicks):00}");
        }
        else if (GameTimeModel.GetHours(GameTimeModel.EnviroTimeOfDayTicks) >= 0 && GameTimeModel.GetHours(GameTimeModel.EnviroTimeOfDayTicks) <= 12)
        {
            View.SetTimeText($"{6 - GameTimeModel.GetHours(GameTimeModel.EnviroTimeOfDayTicks):00}:{60 - GameTimeModel.GetMinutes(GameTimeModel.EnviroTimeOfDayTicks):00}");
        }
    }
}
