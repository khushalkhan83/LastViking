using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Controllers;
using Core;
using Game.Models;
using Game.Views;
using Core.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FishingPullController : IController, IFishingPullController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public FishingModel fishingModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public MiniGameStateModel minigameState { get; private set; }
        [Inject] public FPManager FPManager { get; private set; }
        [Inject] public EventLootModel EventLootModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public FishHealthModel FishHealthModel { get; private set; }


        public IView View { get; private set; }
        public IView FishHealthView { get; private set; }

        public void Enable()
        {
            fishingModel.OnHookResult += BeginPulling;
            fishingModel.OnPull += SetIsPulling;
            minigameState.OnStateChange += OnMiniGameStateChange;
            fishingModel.OnEndOfFishing += OnFishRecive;
        }

        public void Disable()
        {
            fishingModel.OnHookResult -= BeginPulling;
            fishingModel.OnPull -= SetIsPulling;
            minigameState.OnStateChange -= OnMiniGameStateChange;
            fishingModel.OnEndOfFishing -= OnFishRecive;
        }
        public void Start() { }

        void OnMiniGameStateChange(bool isMinigame)
        {
            if (!isMinigame && fishingModel.state == FishingModel.FishingState.pulling)
                FailTorn();
        }

        void BeginPulling(bool isStart)
        {
            if (isStart)
            {
                FishHealthModel.ResetHealth();
                _damageTimer = FishHealthModel.DamageDelay;
                _isOnPull = false;
                _currentMoveSpeed = 0;
                _currentAcceleration = fishingModel.initAcceleration;
                _currentPosition = fishingModel.initPosition;

                float divVal = Random.value * 0.5f;

                _addMultipler = 1f + divVal;
                _reduceMultipler = 1f - divVal;

                OpenView();
                GameUpdateModel.OnUpdate += UpdateState;
            }
        }

        void SetIsPulling(bool isPuuling)
        {
            _isOnPull = isPuuling;
        }

        float _currentMoveSpeed = 0;
        bool _isOnPull = false;
        float _currentAcceleration = 0;
        float _currentPosition = 0;

        float _addMultipler = 1f;
        float _reduceMultipler = 1f;
        private float _damageTimer = 0;

        void UpdateState()
        {
            if (fishingModel.state == FishingModel.FishingState.pulling)
            {
                _damageTimer -= Time.deltaTime;

                if(_damageTimer <= 0 )
                {
                    DoDamage();
                }

                if (FishHealthModel.FishHealth <= 0)
                    Win();
                else
                {
                    _currentAcceleration += fishingModel.initAccelerationAcceleration * Time.deltaTime;
                    if (_isOnPull)
                        _currentMoveSpeed += _currentAcceleration * Time.deltaTime * _addMultipler;
                    else
                        _currentMoveSpeed -= _currentAcceleration * Time.deltaTime * _reduceMultipler;

                    _currentPosition += _currentMoveSpeed * Time.deltaTime;

                    if (_currentPosition < 0f)
                        FailEscape();
                    else if (_currentPosition > 1f)
                        FailTorn();
                    else
                        fishingModel.SetTension(_currentPosition);
                }
            }
        }

        private void DoDamage()
        {
            float damageMultipliyer = 1f - Mathf.Abs(0.5f - _currentPosition) * 2f;
            int damage = Mathf.CeilToInt(damageMultipliyer * FishHealthModel.Damage);

            FishHealthModel.DoDamage(damage);
            _damageTimer = FishHealthModel.DamageDelay;
        }

        void FailEscape()
        {
            fishingModel.FailEscape();
            Finish();
        }

        void FailTorn()
        {
            fishingModel.FailTorn();
            Finish();
        }

        void Win()
        {
            CloseView();
            SetFishedReward();
            fishingModel.FisingSuccessful(); 
        }

        private void SetFishedReward()
        {
            EventLootModel.GetLootFromCustom(fishingModel.fishKey, 150, out string item);
            fishingModel.GivenItem = item;
        }

        void OnFishRecive()
        {
            GiveFish();
            Finish();
            if (FPManager.ItemCurrent.TryGetProperty("Durability", out var rod))
            {
                rod.Float.Current--;
                FPManager.ItemCurrent.SetProperty(rod);
            }
            else
            {
                Debug.LogError("Property Durability not found");
            }
        }

        void GiveFish()
        {
            var item = ItemsDB.GetItem(fishingModel.GivenItem);
            if (item == null) return;

            InventoryOperationsModel.AddItemToPlayer(item.Name,1);
        }

        void Finish()
        {
            CloseView();
            GameUpdateModel.OnUpdate -= UpdateState;
            minigameState.IsMinigame = false;
        }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<FishingPullView>(ViewConfigID.FishingPulling);
                View.OnHide += OnHideHandler;
            }

            if(FishHealthView == null)
            {
                FishHealthView = ViewsSystem.Show<FishHealthView>(ViewConfigID.FishHealthConfig);
                FishHealthView.OnHide += OnHideFishHealthHandler;   
            }
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }

            if (FishHealthView != null)
            {
                ViewsSystem.Hide(FishHealthView);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }

        private void OnHideFishHealthHandler(IView view)
        {
            FishHealthView.OnHide -= OnHideFishHealthHandler;
            FishHealthView = null;
        }
    }
}