using Game.Views;
using Core.Controllers;
using Core;
using Game.Audio;
using System;
using Game.Models;

namespace Game.Controllers
{
    // TODO: remove this and its view (temp)
    public class ProtectShelterPopupViewController : ViewControllerBase<ProtectShelterPopupView>
    {
        [Inject] public AudioSystem AudioSystem {get; private set;}
        [Inject] public ViewsSystem ViewsSystem {get; private set;}
        [Inject] public ShelterAttackModeModel ShelterAttackModeModel {get; private set;}
        protected override void Show() 
        {
            View.OnOk += StartShelterAttack;
            View.OnCancel += Close;
        }

        protected override void Hide() 
        {
            View.OnOk -= StartShelterAttack;
            View.OnCancel -= Close;
        }

        private void StartShelterAttack()
        {
            AudioSystem.PlayOnce(AudioID.Button);

            ShelterAttackModeModel.StartAttackMode();

            Close();
        }

        private void Close()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }
    }
}
