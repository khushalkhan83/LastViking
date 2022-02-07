using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class CampFireCursorViewController : ViewControllerBase<CampFireCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }


        public CampFireModel CampFireModel { get; private set; }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            View.OnInteractAlternative += OnClickFireHandler;

            CampFireModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<CampFireModel>();
            CampFireModel.OnChangeFireState += OnChangeFireStateHandler;
            UpdateFireIconState();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            View.OnInteractAlternative -= OnClickFireHandler;

            CampFireModel.OnChangeFireState -= OnChangeFireStateHandler;
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

        private void UpdateFireIconState()
        {
            SetFireIcon(CampFireModel.IsFire);
        }

        private void OnClickFireHandler()
        {
            CampFireModel.SetFire(!CampFireModel.IsFire);
        }

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
    }
}
