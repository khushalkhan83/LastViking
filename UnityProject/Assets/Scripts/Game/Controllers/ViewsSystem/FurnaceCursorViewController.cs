using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FurnaceCursorViewController : ViewControllerBase<FurnaceCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        

        protected FurnaceModel FurnaceModel { get; private set; }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            View.OnInteractAlternative += OnClickFireHandler;

            FurnaceModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<FurnaceModel>();
            FurnaceModel.OnChangeFireState += OnChangeFireStateHandler;
            UpdateFireIconState();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            View.OnInteractAlternative -= OnClickFireHandler;

            FurnaceModel.OnChangeFireState -= OnChangeFireStateHandler;
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

        private void UpdateFireIconState()
        {
            SetFireIcon(FurnaceModel.IsFire);
        }

        private void OnClickFireHandler()
        {
            FurnaceModel.SetFire(!FurnaceModel.IsFire);
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

        private void OnClickOpenHandler()
        {
            ViewsSystem.Show<FurnaceView>(ViewConfigID.Furnace);
        }
    }
}
