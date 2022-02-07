using Game.Views;
using Core.Controllers;
using Game.Models;
using System.Collections;
using Core;
using UnityEngine;
using System;

namespace Game.Controllers
{
    public class ZoneMessageViewController : ViewControllerBase<ZoneMessageView>
    {
        [Inject] public ViewsSystem ViewsSystem {get;set;}
        [Inject] public ZoneModel ZoneModel {get;set;}
        [Inject] public LocalizationModel LocalizationModel {get;set;}

        protected override void Show() 
        {
            View.SetMessateText(GetCurrentZoneName());
            View.PlayShowAnimatoin();
            StartCoroutine(HideView());
        }

        protected override void Hide() 
        {
            StopAllCoroutines();
        }

        // TODO: add error handling or check on build localization id and ids match
        private string GetCurrentZoneName()
        {
            // return LocalizationModel.GetString((LocalizationKeyID)Enum.Parse(typeof(LocalizationKeyID),ZoneModel.PlayerZone.ToString()));
            return ZoneModel.PlayerZone;
        }

        private IEnumerator HideView()
        {
            yield return new WaitForSeconds(4f);
            View.PlayHideAnimation(OnHideAnimationComplete);
        }

        private void OnHideAnimationComplete()
        {
            ViewsSystem.Hide(View);
        }

    }
}
