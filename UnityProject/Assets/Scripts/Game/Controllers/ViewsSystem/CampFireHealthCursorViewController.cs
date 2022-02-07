using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class CampFireHealthCursorViewController : ViewControllerBase<CampFireHealthCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected CampFireModel CampFireModel { get; private set; }
        protected RestorableObject RestorableObject { get; private set; }


        protected override void Show()
        {
            View.OnInteractAlternative += OnClickFireHandler;
            GameUpdateModel.OnUpdate += OnUpdate;

            CampFireModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<CampFireModel>();
            CampFireModel.OnChangeFireState += OnChangeFireStateHandler;
            UpdateFireIconState();

            RestorableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<RestorableObject>();
            RestorableObject.Health.OnChangeHealth += OnChangeHealthHandler;
            UpdateView();
        }

        protected override void Hide()
        {
            View.OnInteractAlternative -= OnClickFireHandler;
            GameUpdateModel.OnUpdate -= OnUpdate;

            CampFireModel.OnChangeFireState -= OnChangeFireStateHandler;
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
            if (CampFireModel.IsFire == false)
            {
                AudioSystem.PlayOnce(AudioID.TurnOffFire);
            }
            else
            {
                AudioSystem.PlayOnce(AudioID.TurnOnFire);
            }
        }

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateFireIconState() => SetFireIcon(CampFireModel.IsFire);

        private void OnClickFireHandler() => CampFireModel.SetFire(!CampFireModel.IsFire);

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

        private void OnClickOpenHandler() => ViewsSystem.Show<CampFireView>(ViewConfigID.CampFire);

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetFillAmount(health / healthMax);
            View.SetText($"{health:F0}/{healthMax:F0}");
        }
    }
}
