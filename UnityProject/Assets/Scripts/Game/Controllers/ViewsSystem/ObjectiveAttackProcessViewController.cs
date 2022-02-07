using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

public class ObjectiveAttackProcessViewController : ViewControllerBase<ObjectiveAttackProcessView>
{
    [Inject] public SheltersModel SheltersModel { get; private set; }
    [Inject] public GameTimeModel GameTimeModel { get; private set; }
    [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
    [Inject] public SkeletonSpawnManager SkeletonSpawnManager { get; private set; }

    protected ShelterModel ShelterModel { get; private set; }
    protected IHealth HealthCurrent { get; private set; }

    protected override void Show()
    {
        View.PlayShow();

        if (SheltersModel.ShelterActive == ShelterModelID.None)
        {
            SheltersModel.OnActivate += OnActivateShelter;
        }
        else
        {
            OnActivateShelter(SheltersModel.ShelterModel);
        }
        GameUpdateModel.OnUpdate += OnUpdate;
    }

    protected override void Hide()
    {
        View.PlayHide(); // don't work because view isn't ViewAnimationBase

        SheltersModel.OnActivate -= OnActivateShelter;
        if (ShelterModel)
        {
            ShelterModel.OnUpgrade -= OnUpgradeShelterHandler;
        }

        GameUpdateModel.OnUpdate -= OnUpdate;
    }

    private void OnActivateShelter(ShelterModel shelterModel)
    {
        ShelterModel = SheltersModel.ShelterModel;

        ShelterModel.OnUpgrade += OnUpgradeShelterHandler;

        HealthCurrent = ShelterModel.GetComponentInChildren<IHealth>();
        HealthCurrent.OnChangeHealth += OnChangeHealthHandler;

        UpdateShelterHealthAmount();
    }

    private void OnChangeHealthHandler() => UpdateShelterHealthAmount();

    private void UpdateShelterHealthAmount()
    {
        View.SetFillAmount(HealthCurrent.Health / HealthCurrent.HealthMax);
        View.SetSliderValue(HealthCurrent.Health / HealthCurrent.HealthMax);
        View.SetShelterHPText($"{HealthCurrent.Health} / {HealthCurrent.HealthMax}");
    }

    private void OnUpgradeShelterHandler()
    {
        HealthCurrent.OnChangeHealth -= OnChangeHealthHandler;
        HealthCurrent = null;

        HealthCurrent = ShelterModel.GetComponentInChildren<IHealth>();
        HealthCurrent.OnChangeHealth += OnChangeHealthHandler;

        UpdateShelterHealthAmount();
    }

    //[UnityEngine]
    private SkeletonSpawnManager.SessionSettings __session;
    private SkeletonSpawnManager.WaveSettings __wave;

    private void OnUpdate()
    {
        if (SkeletonSpawnManager.TryGetSession(out __session))
        {
            View.SetZombiesAliveText($"{SkeletonSpawnManager.KilledEnemiesPerSession} / {SkeletonSpawnManager.TotalSessionEnemies}");
        }

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
