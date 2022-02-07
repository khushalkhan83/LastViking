using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class LoomCursorViewController : ViewControllerBase<LoomCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected RestorableObject RestorableObject { get; private set; }
        protected LoomModel LoomModel { get; private set; }


        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            View.OnInteractAlternative += OnClickWeaveHandler;

            LoomModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<LoomModel>();
            LoomModel.OnChangeWeaveState += OnChangeWeaveStateHandler;
            UpdateWeaveIconState();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            View.OnInteractAlternative -= OnClickWeaveHandler;

            LoomModel.OnChangeWeaveState -= OnChangeWeaveStateHandler;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnClickOpenHandler();
            }
        }

        private void OnChangeWeaveStateHandler()
        {
            UpdateWeaveIconState();
            if (LoomModel.IsWeave == false)
            {
                AudioSystem.PlayOnce(AudioID.TurnOffLoom);
            }
            else
            {
                AudioSystem.PlayOnce(AudioID.TurnOnLoom);
            }
        }

        private void UpdateWeaveIconState() => SetWeaveIcon(LoomModel.IsWeave);

        private void OnClickWeaveHandler() => LoomModel.SetWeave(!LoomModel.IsWeave);

        private void SetWeaveIcon(bool isFire)
        {
            if (isFire)
            {
                View.SetWeaveIcon(View.WeaveOn);
            }
            else
            {
                View.SetWeaveIcon(View.WeaveOff);
            }
        }

        private void OnClickOpenHandler() => ViewsSystem.Show<LoomView>(ViewConfigID.Loom);
    }
}
