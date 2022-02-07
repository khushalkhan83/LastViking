using System;
using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.Views;
using QuestEvent = Game.Models.QuestsLifecycleModel.QuestEvent;

namespace Game.Controllers
{
    public class ShelterUpgradeController : IShelterUpgradeController, IController
    {
        [Inject] public ShelterUpgradeModel ShelterUpgradeModel {get; private set;}
        [Inject] public ShelterAttackModeModel ShelterAttackModeModel {get; private set;}
        [Inject] public ViewsSystem ViewsSystem {get; private set;}
        [Inject] public QuestsModel QuestsModel {get; private set;}
        [Inject] public SheltersModel SheltersModel {get; private set;}
        [Inject] public ShelterModelsProvider ShelterModelsProvider {get; private set;}
        [Inject] public GameTimeModel GameTimeModel {get; private set;}
        [Inject] public InventoryOperationsModel InventoryOperationsModel {get; private set;}
        [Inject] public QuestsLifecycleModel QuestsLifecycleModel {get; private set;}

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private bool ShelterExist => SheltersModel.ShelterModel != null && SheltersModel.ShelterActive != ShelterModelID.None;
        private int RealShelterLevel => ShelterExist ? SheltersModel.ShelterModel.Level: 0;
        private bool NextLevelIsTargetLevel => QuestsModel.TargetShipLevel == RealShelterLevel + 1;
        private bool NextLevelHigherThenTargetLevel => RealShelterLevel + 1 > QuestsModel.TargetShipLevel;

        private bool EnemiesAttackWillStartOnUpgrade => ShelterUpgradeModel.EnemiesAttackWillStartOnUpgrade;
        private ShelterModel ShipShelterModel => ShelterModelsProvider[ShelterModelID.Ship];
        private bool IgnorePriceForUpgrade => EditorGameSettings.IgnoreItemsPrice;

        void IController.Enable() 
        {
            ShelterUpgradeModel.OnGetCanBeUpgraded += GetCanBeUpgradedHandler;
            ShelterUpgradeModel.OnGetEnemiesAttackWillStartOnUpgrade += GetEnemiesAttackWillStartOnUpgrade;
            ShelterUpgradeModel.OnGetNeedQuestItem += GetNeedQuestItem;
            ShelterUpgradeModel.OnGetUpgradedInThisChapter += GetUpgradedInThisChapter;
            ShelterUpgradeModel.OnGetIsConstructed += GetIsConstructed;
            ShelterUpgradeModel.OnInteractWithShelterUpgradeTable += OnInteractWithShelterUpgradeTableHandler;
            ShelterUpgradeModel.OnStartUpgrade += OnStartUpgrade;
            ShelterUpgradeModel.OnCompleteUpgrade += OnCompleteUpgrade;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            ShelterUpgradeModel.OnGetCanBeUpgraded -= GetCanBeUpgradedHandler;
            ShelterUpgradeModel.OnGetEnemiesAttackWillStartOnUpgrade -= GetEnemiesAttackWillStartOnUpgrade;
            ShelterUpgradeModel.OnGetNeedQuestItem -= GetNeedQuestItem;
            ShelterUpgradeModel.OnGetUpgradedInThisChapter -= GetUpgradedInThisChapter;
            ShelterUpgradeModel.OnGetIsConstructed -= GetIsConstructed;
            ShelterUpgradeModel.OnInteractWithShelterUpgradeTable -= OnInteractWithShelterUpgradeTableHandler;
            ShelterUpgradeModel.OnStartUpgrade -= OnStartUpgrade;
            ShelterUpgradeModel.OnCompleteUpgrade -= OnCompleteUpgrade;
        }

        private void OnInteractWithShelterUpgradeTableHandler()
        {
            if(ShelterAttackModeModel.AttackModeActive) return;
            if(QuestsLifecycleModel.EventOccured(QuestEvent.NoUpgradesLeft)) return;

            //TODO: remove ProtectShelterPopupConfig and its view controller
            // if(QuestsModel.PlayerCanStartShelterDefenceMode)
            // {
            //     ViewsSystem.Show<ProtectShelterPopupView>(ViewConfigID.ProtectShelterPopupConfig);
            // }
            // else
            // {
            //     ViewsSystem.Show<ShelterPopupView>(ViewConfigID.ShelterPopup);
            // }
            
            ViewsSystem.Show<ShelterPopupView>(ViewConfigID.ShelterPopup);
        }

        private bool GetCanBeUpgradedHandler()
        {
            if(!NextLevelIsTargetLevel)
            {
                // if target level is 7 then we have no restrictions to upgrade from 0 to 7
                if(NextLevelHigherThenTargetLevel)
                    return false;
                else
                    return true;
            }
            else
            {
                if(ShelterAttackModeModel.AttackModeActive) return false;
                if(QuestsModel.UpgradeStage == false) return false;

                return true;
            }
        }

        private bool GetEnemiesAttackWillStartOnUpgrade()
        {
            bool attack = NextLevelIsTargetLevel && ShelterAttackModeModel.AttackModeAvaliable;
            return attack;
        }

        private bool GetNeedQuestItem()
        {
            bool needQuestItem = QuestsModel.QuestItemData != null && NextLevelIsTargetLevel;
            return needQuestItem;
        }

        private bool GetUpgradedInThisChapter()
        {
            bool wasUpgraded = RealShelterLevel >= QuestsModel.TargetShipLevel;
            return wasUpgraded;
        }

        private bool GetIsConstructed()
        {
            return ShelterExist;
        }

        private void OnStartUpgrade()
        {
            #region Logic
                
            RemoveCost();
            InitUpgrade();

            #endregion

            #region Methods
                
            void RemoveCost()
            {
                if (SheltersModel.ShelterActive == ShelterModelID.None)
                {
                    ShelterModel shipShelterModel = ShelterModelsProvider[ShelterModelID.Ship];
                    RemoveFromPlayer(shipShelterModel.CostBuy.Costs);
                }
                else
                {
                    RemoveFromPlayer(SheltersModel.ShelterModel.CostUpgradeCurrent.Costs);
                }
            }

            void InitUpgrade()
            {
                if (EnemiesAttackWillStartOnUpgrade)
                {
                    ShelterAttackModeModel.StartAttackMode();
                }
                else
                {
                    ShelterUpgradeModel.PreCompleateUpgrade();
                }
            }

            #endregion
            
        }

        private void OnCompleteUpgrade()
        {
            RealUpgrade();
        }

        private void RealUpgrade()
        {
            if (SheltersModel.ShelterActive == ShelterModelID.None)
            {
                SheltersModel.SetShelter(ShipShelterModel);
                SheltersModel.ShelterModel.Buy(GameTimeModel.Ticks);
                SheltersModel.ShelterModel.Activate();
            }
            else
            {
                SheltersModel.ShelterModel.Upgrade();
            }
        }

        private void RemoveFromPlayer(IEnumerable<ShelterCost> costs)
        {
            if(IgnorePriceForUpgrade) return;
            
            foreach (var item in costs)
            {
                RemoveFromPlayer(item);
            }
        }

        private void RemoveFromPlayer(ShelterCost item)
        {
            InventoryOperationsModel.RemoveItemFromPlayer(item.Name, item.Count);
        }
    }
}
