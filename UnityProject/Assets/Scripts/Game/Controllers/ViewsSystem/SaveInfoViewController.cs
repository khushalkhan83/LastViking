using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class SaveInfoViewController : ViewControllerBase<SaveInfoView>
    {
        [Inject] public AutosaveModel AutosaveModel { get; private set; }

        protected override void Show()
        {
            View.SetDescriptionText("Save time: " + AutosaveModel.LastSaveTime.ToString("h:mm:ss"));
        }

        protected override void Hide()
        {
            
        }
    }
}
