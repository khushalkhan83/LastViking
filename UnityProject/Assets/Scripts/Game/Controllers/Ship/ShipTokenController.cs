using System.Collections;
using System.Collections.Generic;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using UnityEngine;

namespace Game.Controllers
{
    public class ShipTokenController : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private TokenTarget normalToken;
        [SerializeField] private TokenTarget upgradeToken;
        
        #pragma warning restore 0649
        #endregion

        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private TokensModel TokensModel => ModelsSystem.Instance._tokensModel;
        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        private TokenTarget NoramlToken => normalToken;
        private TokenTarget UpgradeToken => normalToken;

        #region MonoBehaviour
        private void OnEnable() 
        {
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }
        private void OnDisable() 
        {
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
            UnSubscribe();
        }
        #endregion

        private void Subscribe()
        {
            SheltersModel.OnBuy += OnBuyShelter;
            QuestsModel.OnActivateStage += UpdateToken;
            ShelterAttackModeModel.OnAttackModeActiveChanged += UpdateToken;
        }

        private void UnSubscribe()
        {
            SheltersModel.OnBuy -= OnBuyShelter;
            QuestsModel.OnActivateStage -= UpdateToken;
            ShelterAttackModeModel.OnAttackModeActiveChanged -= UpdateToken;
        }

        private void OnEnvironmentLoaded()
        {
            UnSubscribe();
            Subscribe();
            UpdateToken();
        }

        private void OnBuyShelter( ShelterModel shelterModel) => UpdateToken();

        private void UpdateToken()
        {
            if(SheltersModel.IsBuyed(ShelterModelID.Ship))
            {
                bool isUpradeStage = QuestsModel.UpgradeStage && !ShelterAttackModeModel.AttackModeActive;
                NoramlToken.enabled = !isUpradeStage;
                UpgradeToken.enabled = isUpradeStage;
            }
            else
            {
                NoramlToken.enabled = false;
                UpgradeToken.enabled = false;
            }
        }
    }
}
