using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class DiscordButtonViewController : ViewControllerBase<DiscordButtonView>
    {
        [Inject] public DiscordModel DiscordModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public DiscordButtonViewModel ViewModel { get; private set; }

        protected override void Show()
        {
            View.OnClick += HandleButtonClick;
            View.transform.SetAsLastSibling();
        }

        protected override void Hide()
        {
            View.OnClick -= HandleButtonClick;
        }

        private void HandleButtonClick()
        {
            ViewModel.Click();
            AudioSystem.PlayOnce(AudioID.Button);
            Application.OpenURL(DiscordModel.URL);
        }
    }
}
