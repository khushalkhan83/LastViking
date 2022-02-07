using Core;
using Core.Controllers;
using Game.AI;
using Game.Models;
using Game.Providers;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.Views;
using System;
using UnityEngine;

namespace Game.Controllers
{
    public class AutosaveController : IAutosaveController, IController
    {
        [Inject] public ShelterModelsProvider ShelterModelsProvider { get; private set; }
        [Inject] public StartNightInfoModel StartNightInfoModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public AutosaveModel AutosaveModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public AnimalsModel AnimalsModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public EnoughSpaceModel EnoughSpaceModel{ get; set; }

        private bool HasShelter => SheltersModel.ShelterActive != ShelterModelID.None;

        private ShelterModel ShipShelterModel => ShelterModelsProvider[ShelterModelID.Ship];

        public ulong LastSavedHour { private set; get; }
        public bool HasSavedTiming { private set; get; }

        void IController.Enable()
        {
            StorageModel.OnSaveChanged += OnSaveChanged;

            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
            ApplicationCallbacksModel.ApplicationQuit += OnApplicationQuit;

            if (AutosaveModel.SaveOnEvents)
            {
                PlayerScenesModel.OnEnvironmentChange += OnEnvironmentChange;
                PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
                ShipShelterModel.OnBuy += OnBuyShipShelter;
                ShipShelterModel.OnPreUpgrade += OnPreUpgradeShipShelter;
                StartNightInfoModel.OnStartNight += OnStartNight;
                StartNightInfoModel.OnEndNight += OnEndNight;
                AnimalsModel.OnTargetKillAnimal += OnPlayerKillAnimal;
                PurchasesModel.OnPurchaseSuccessfully += OnPurchase;
                GameUpdateModel.OnUpdate += UpdateChangedSave;

                if (!HasShelter)
                {
                    SheltersModel.OnBuy += OnBuyShipShelterIfHasnt;
                    GameUpdateModel.OnUpdate += UpdateTimingSave;
                }
                else
                {
                    SheltersModel.OnDeath += OnDeathShelterIfHas;
                }
            }
        }

        void IController.Start()
        {
    
        }

        void IController.Disable()
        {
            StorageModel.OnSaveChanged -= OnSaveChanged;

            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
            ApplicationCallbacksModel.ApplicationQuit -= OnApplicationQuit;

            if (AutosaveModel.SaveOnEvents)
            {
                PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentChange;
                PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
                ShipShelterModel.OnBuy -= OnBuyShipShelter;
                ShipShelterModel.OnPreUpgrade -= OnPreUpgradeShipShelter;
                StartNightInfoModel.OnStartNight -= OnStartNight;
                StartNightInfoModel.OnEndNight -= OnEndNight;
                AnimalsModel.OnTargetKillAnimal -= OnPlayerKillAnimal;
                PurchasesModel.OnPurchaseSuccessfully -= OnPurchase;
                GameUpdateModel.OnUpdate -= UpdateChangedSave;

                SheltersModel.OnBuy -= OnBuyShipShelterIfHasnt;
                GameUpdateModel.OnUpdate -= UpdateTimingSave;
                SheltersModel.OnDeath -= OnDeathShelterIfHas;
            }

            // SaveChanged();
        }

        private void OnSaveChanged() => AutosaveModel.Save();

        private void OnEnvironmentChange() => SaveChanged();

        private void OnEnvironmentLoaded() => SaveChanged();

        private float _tickChanged = 0;
        private void UpdateChangedSave()
        {
            _tickChanged += Time.deltaTime;
            if (_tickChanged >= AutosaveModel.SaveInterval)
            {
                _tickChanged = 0;
                TryAutosaveChanged();
            }
        }

        private void UpdateTimingSave()
        {
            var hours = GameTimeModel.Hours;
            if (IsHourForSave((int)hours))
            {
                if (!HasSavedTiming)
                {
                    LastSavedHour = hours;
                    HasSavedTiming = true;
                    SaveChanged();
                }
            }
            if (HasSavedTiming && hours != LastSavedHour)
            {
                HasSavedTiming = false;
            }
        }

        private bool IsHourForSave(int hours) => Array.IndexOf(AutosaveModel.SaveHours, hours) != -1;

        private void OnBuyShipShelter() => SaveChanged();

        private void OnPreUpgradeShipShelter()
        {
            ShipShelterModel.SetIsChangeLevel(true);
            SaveChanged();
        }

        private void OnStartNight() => SaveChanged();

        private void OnEndNight() => SaveChanged();

        private void OnPlayerKillAnimal(Target target, AnimalID animalID)
        {
            if (animalID == AnimalID.Bear || animalID == AnimalID.Boar || animalID == AnimalID.Wolf)
                SaveChanged();
        }

        private void OnPurchase(PurchaseID purchaseID)
        {
            if (PurchasesModel.GetInfo<IPurchaseStoreInfo>(purchaseID) != null ||
                PurchasesModel.GetInfo<GoldenPackWatch>(purchaseID) != null)
                SaveChanged();
        }

        private void OnBuyShipShelterIfHasnt(ShelterModel shelter)
        {
            SheltersModel.OnBuy -= OnBuyShipShelterIfHasnt;
            GameUpdateModel.OnUpdate -= UpdateTimingSave;
            SheltersModel.OnDeath += OnDeathShelterIfHas;
        }

        private void OnDeathShelterIfHas(ShelterModel shelter)
        {
            SheltersModel.OnDeath -= OnDeathShelterIfHas;
            SheltersModel.OnBuy += OnBuyShipShelterIfHasnt;
            GameUpdateModel.OnUpdate += UpdateTimingSave;
        }

        private bool TryAutosaveChanged()
        {
            if(EnoughSpaceModel.HasEnoughSpace(out bool criticalyNotEnough))
            {
                StorageModel.SaveChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SaveChanged()
        {
            try
            {
                StorageModel.SaveChanged();

                if (ShipShelterModel.IsChangeLevel) // [WTF]
                {
                    ShipShelterModel.Upgraded();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(new Exception($"Error on save Changed {ex.ToString()}"));
                throw;
            }
        }

        private void SaveLate()
        {
            StorageModel.SaveLateSavable();
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveChanged();
                SaveLate();
            }
        } 

        private void OnApplicationQuit()
        {
            SaveChanged();
            SaveLate();
        }

        private void OnApplicationFocus(bool focus)
        {
#if !UNITY_EDITOR
            if (!focus)
            {
                SaveChanged();
                SaveLate();
            }
#endif
        }
    }
}
