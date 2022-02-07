using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Providers;
using Game.Views;
using UnityEngine;
using QuestEvent = Game.Models.QuestsLifecycleModel.QuestEvent;

namespace Game.Controllers
{
    public class ShelterUpgradeTableController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [ObscuredID(typeof(ShelterModelID))]
        [SerializeField] private ObscuredInt _shelterModelID;

        [SerializeField] private ShelterUpgradeTableView _shelterUpgradeTableView;

#pragma warning restore 0649
        #endregion

        public ShelterUpgradeTableView ShelterUpgradeTableView => _shelterUpgradeTableView;
        private ConstructionDockModel ConstructionDockModel => ModelsSystem.Instance._constructionDockModel;
        private QuestsLifecycleModel QuestsLifecycleModel => ModelsSystem.Instance._questsLifecycle;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        private bool MainQuestEnded => QuestsLifecycleModel.EventOccured(QuestEvent.NoUpgradesLeft);

        #region MonoBehaviour
        private void Start()
        {
            UpdateTableView();
        }

        private void OnEnable()
        {
            ConstructionDockModel.NeedBuildDocChanged += NeedBuildDocChanged;
            ConstructionDockModel.OnDockBuilded += UpdateTableView;
            QuestsModel.OnActivateStage += UpdateTableView;
        }

        private void OnDisable()
        {
            ConstructionDockModel.NeedBuildDocChanged -= NeedBuildDocChanged;
            ConstructionDockModel.OnDockBuilded -= UpdateTableView;
            QuestsModel.OnActivateStage -= UpdateTableView;
        }
        #endregion

        private void UpdateTableView()
        {
            if(ConstructionDockModel.DockBuilded && !MainQuestEnded) Show();
            else Hide();
        }

        private void NeedBuildDocChanged(bool value) => UpdateTableView();


        private void Show()
        {
            ShelterUpgradeTableView.SetIsVisible(true);
        }

        private void Hide()
        {
            ShelterUpgradeTableView.SetIsVisible(false);
        }
    }
}
