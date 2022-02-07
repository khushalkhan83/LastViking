using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FurnaceHealthCursorViewController : ViewControllerBase<FurnaceHealthCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected RestorableObject RestorableObject { get; private set; }
        protected FurnaceModel FurnaceModel { get; private set; }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            View.OnClickFire += OnClickFireHandler;

            FurnaceModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<FurnaceModel>();
            FurnaceModel.OnChangeFireState += OnChangeFireStateHandler;
            UpdateFireIconState();

            RestorableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<RestorableObject>();
            RestorableObject.Health.OnChangeHealth += OnChangeHealthHandler;
            UpdateView();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            View.OnClickFire -= OnClickFireHandler;

            FurnaceModel.OnChangeFireState -= OnChangeFireStateHandler;
            RestorableObject.Health.OnChangeHealth -= OnChangeHealthHandler;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnClickOpenHandler();
            }
        }

        private void OnChangeFireStateHandler()
        {
            UpdateFireIconState();
            if (FurnaceModel.IsFire == false)
            {
                AudioSystem.PlayOnce(AudioID.TurnOffFire);
            }
            else
            {
                AudioSystem.PlayOnce(AudioID.TurnOnFire);
            }
        }

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateFireIconState() => SetFireIcon(FurnaceModel.IsFire);

        private void OnClickFireHandler() => FurnaceModel.SetFire(!FurnaceModel.IsFire);

        private void SetFireIcon(bool isFire)
        {
            if (isFire)
            {
                View.SetFireIcon(View.FireOn);
            }
            else
            {
                View.SetFireIcon(View.FireOff);
            }
        }

        private void OnClickOpenHandler() => ViewsSystem.Show<FurnaceView>(ViewConfigID.Furnace);

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetFillAmount(health / healthMax);
            View.SetText($"{health:F0}/{healthMax:F0}");
        }
    }
}
