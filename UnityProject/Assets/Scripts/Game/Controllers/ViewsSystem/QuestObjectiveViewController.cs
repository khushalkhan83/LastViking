using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using Extensions;

namespace Game.Controllers
{
    public class QuestObjectiveViewController : ViewControllerBase<QuestObjectiveView>
    {
        [Inject] public QuestNotificationsModel QuestNotificationsModel {get; private set;}

        private bool isMessageDisplayed;
        protected override void Show() 
        {
            QuestNotificationsModel.OnShow += ShowMessage;
            QuestNotificationsModel.OnHide += HidePopup;
            QuestNotificationsModel.OnSetTargetEnemyHealth += UpdateTargetEnemyHealth;
            QuestNotificationsModel.OnSetTargetItemsCount += UpdateTargetItemsData;
            QuestNotificationsModel.OnClearTargetEnemyHealth += OnClearedTargetEnemyHealth;
        }

        protected override void Hide() 
        {
            QuestNotificationsModel.OnShow -= ShowMessage;
            QuestNotificationsModel.OnHide -= HidePopup;
            QuestNotificationsModel.OnSetTargetEnemyHealth -= UpdateTargetEnemyHealth;
            QuestNotificationsModel.OnSetTargetItemsCount -= UpdateTargetItemsData;
            QuestNotificationsModel.OnClearTargetEnemyHealth -= OnClearedTargetEnemyHealth;
        }

        private void ShowMessage()
        {
            UpdateView();
            View.PlayShowTop();
            isMessageDisplayed = true;
        }

        private void UpdateView()
        {
            View.SetObjecvtiveIcon(QuestNotificationsModel.Icon);
            View.SetDescriptionText(QuestNotificationsModel.Message);

            switch (QuestNotificationsModel.NotificationType)
            {
                case QuestNotificationsModel.State.Default:
                    ShowExtraFields(false);
                    break;
                case QuestNotificationsModel.State.ItemsCount:
                    UpdateTargetItemsData();
                    break;
                case QuestNotificationsModel.State.TargetHealth:
                    UpdateTargetHealthData();
                    break;
                default:
                    break;
            }
        }

        private void HidePopup()
        {
            if(isMessageDisplayed)
            {
                View.CheckNull()?.PlayHideTop();
                isMessageDisplayed = false;
            }
            else
            {
                View.CheckNull()?.PlayHidden();
            }
        }

        #region Enemy health display
            
        private void UpdateTargetEnemyHealth()
        {
            var targetEnemyHealth = QuestNotificationsModel.TargetEnemyHealth;
            targetEnemyHealth.OnChangeHealth += OnChangeHealth;
            targetEnemyHealth.OnDeath += OnTargetDeath;

            OnChangeHealth();
        }

        private void OnChangeHealth() => UpdateTargetHealthData();

        private void UpdateTargetHealthData()
        {
            ShowExtraFields(true);
            View.ShowTimeText(false); // hide kraken health 

            var targetEnemyHealth = QuestNotificationsModel.TargetEnemyHealth;

            var health = targetEnemyHealth.Health;
            var maxHealth = targetEnemyHealth.HealthMax;

            var fillAmaunt = health/ maxHealth;

            View.SetSliderValue(fillAmaunt);
            View.SetTimeText($"{health}/{maxHealth}");
        }

        private void OnClearedTargetEnemyHealth()
        {
            ShowExtraFields(false);
            var targetEnemyHealth = QuestNotificationsModel.TargetEnemyHealth;
            if(targetEnemyHealth == null) return;

            targetEnemyHealth.OnChangeHealth -= OnChangeHealth;
            targetEnemyHealth.OnDeath -= OnTargetDeath;
        }
        #endregion

        #region Items count display
        private void UpdateTargetItemsData()
        {
            ShowExtraFields(true);

            var have = QuestNotificationsModel.HaveItemsCount;
            var target = QuestNotificationsModel.TargetItemsCount;

            var fillAmaunt = (float)have/ (float)target;

            View.SetSliderValue(fillAmaunt);
            View.SetTimeText($"{have}/{target}");
        }
            
        #endregion


        private void OnTargetDeath() => OnClearedTargetEnemyHealth();

        private void ShowExtraFields(bool show)
        {
            View.ShowFill(show);
            View.ShowSlider(show);
            View.ShowTimeText(show);
        }
    }
}
