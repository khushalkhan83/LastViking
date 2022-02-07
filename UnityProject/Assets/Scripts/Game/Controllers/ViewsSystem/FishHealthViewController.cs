using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class FishHealthViewController : ViewControllerBase<FishHealthView>
    {
        [Inject] public FishingModel fishingModel { get; private set; }
        [Inject] public FishHealthModel FishHealthModel { get; private set; }

        private Camera cameraMain = null;
        private RectTransform canvasRect = null;
        private Transform FishHealthPivot = null;

        protected override void Show()
        {
            cameraMain = Camera.main;
            canvasRect = View.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            FishHealthPivot = FindObjectOfType<FishingFloat>()?.FishHealthPivot;
            FishHealthModel.OnDamage += OnDamage;
            View.SetHealthSlider(1f);
            View.SetHeathText(FishHealthModel.FishHealth.ToString());
        }

        protected override void Hide()
        {
            FishHealthModel.OnDamage -= OnDamage;
        }

        private void OnDamage(int damage)
        {
            float health = FishHealthModel.FishHealth / (float)FishHealthModel.FullFishHealth;
            View.SetHealthSlider(health);
            View.SetHeathText(FishHealthModel.FishHealth.ToString());
            View.ShowDamage(damage);
        }

        private void LateUpdate() 
        {
            UpdateViewScreenPosition();
        }

        private void UpdateViewScreenPosition()
        {
            if(View != null && FishHealthPivot != null)
            {
                Vector2 viewPortPosition = cameraMain.WorldToViewportPoint(FishHealthPivot.position);
                Vector2 screenPosition = new Vector2(
                    ((viewPortPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)),
                    ((viewPortPosition.y*canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y*0.5f)));

                View.SetScreenPosition(screenPosition);
            }
        }

    }
}
