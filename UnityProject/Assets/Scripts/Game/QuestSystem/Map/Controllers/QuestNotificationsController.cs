using System;
using System.Collections;
using Core.Views;
using Extensions;
using Game.Models;
using Game.QuestSystem.Data;
using Game.Views;
using Game.InGameNotifications;
using NaughtyAttributes;
using UnityEngine;

namespace Game.QuestSystem.Map.Controllers
{
    public class QuestNotificationsController : MonoBehaviour
    {
        private QuestNotificationsModel QuestNotificationsModel => ModelsSystem.Instance._questNotificationsModel;
        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;
        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;
        private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;
        private CoroutineModel CoroutineModel => ModelsSystem.Instance._coroutineModel;
        private MedallionsModel MedallionsModel => ModelsSystem.Instance._medallionsModel;
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel; 
        private NotificationsModel NotificationsModel => ModelsSystem.Instance._notificationsModel;
        private CinematicModel CinematicModel => ModelsSystem.Instance._cinematicModel;

        private IView view;
        private bool needToResetViews;
        private int coroutineIndex = -1;
        private int extraCoroutineIndex = -1;
        private const float messageShowTime = 5f;
        private const float showDelayAfterSideQuest = 3f;

        private bool showMainQuestMessage = true;

        private bool HaveMainQuestRegularMessage => QuestsModel.ShowPopup;
        private bool HaveMainQuestUpgradeMessage => ShelterUpgradeModel.NeedQuestItem && ShelterUpgradeModel.CanBeUpgraded && showMainQuestMessage;

        private bool HaveMainQuestMessage => HaveMainQuestMessage || HaveMainQuestUpgradeMessage;

        #region Testing

        [Button] void TesetShow() => QuestNotificationsModel.Show("Hello",QuestsModel.StageIcon);
        [Button] void TesetHide() => QuestNotificationsModel.Hide();
            
        #endregion
 
        #region MonoBehaviour
        private void OnEnable()
        {
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }
        private void OnDisable()
        {
            QuestsModel.OnActivateStage -= OnActivateStage;
            SideQuestsModel.OnStageChanged -= OnSideQuestUpdated;
            PlayerDeathModel.OnPreRevival -= ResetViews;
            MedallionsModel.OnCollect -= OnCollectMedallions;
            LocalizationModel.OnChangeLanguage -= ResetViews;
            NotificationsModel.OnNotificationDataAdded -= OnNotificationDataAdded;
            TutorialModel.OnComplete -= MainLogic;
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;

            QuestNotificationsModel.SetDefaultState();
            QuestNotificationsModel.Hide();
            ViewsSystem.TryHideView(view);

            CoroutineModel.CheckNull()?.BreakeCoroutine(coroutineIndex);
            CoroutineModel.CheckNull()?.BreakeCoroutine(extraCoroutineIndex);
        }
        #endregion


        private void OnEnvironmentLoaded()
        {
            QuestsModel.OnActivateStage += OnActivateStage;
            SideQuestsModel.OnStageChanged += OnSideQuestUpdated;
            PlayerDeathModel.OnPreRevival += ResetViews;
            MedallionsModel.OnCollect += OnCollectMedallions;
            LocalizationModel.OnChangeLanguage += ResetViews;
            NotificationsModel.OnNotificationDataAdded += OnNotificationDataAdded;
            MainLogic();

            if(!TutorialModel.IsComplete)
            {
                TutorialModel.OnComplete += MainLogic;
            }
        }

        private void MainLogic()
        {
            UpdateObjectiveView();
        }

        private void OnActivateStage()
        {
            showMainQuestMessage = true;
            UpdateObjectiveView();
        }

        private void ResetViews()
        {
            needToResetViews = true;
            UpdateObjectiveView();
        }

        private void UpdateObjectiveView()
        {
            CoroutineModel.BreakeCoroutine(coroutineIndex);
            CoroutineModel.BreakeCoroutine(extraCoroutineIndex);

            if(TutorialModel.IsComplete)
            {
                if(view == null || needToResetViews)
                {
                    if(view != null) ViewsSystem.Hide(view);
                    
                    view = ViewsSystem.Show(ViewConfigID.QuestObjective);
                    needToResetViews = false;
                }

                if(!TryShowInGameNotifications())
                {
                    PresentMainQuestNotification();
                }
            }
            else
            {
                QuestNotificationsModel.Hide();
            }

            void PresentMainQuestNotification()
            {
                if(HaveMainQuestRegularMessage)
                    QuestNotificationsModel.Show(QuestsModel.StageDescription,QuestsModel.StageIcon);
                else if (HaveMainQuestUpgradeMessage)
                {
                    showMainQuestMessage = false;
                    QuestNotificationsModel.Show(GetCanUpgradeShelterWithNewItemMessage(), QuestsModel.QuestItemData?.ItemIcon);
                }
                else
                    QuestNotificationsModel.Hide();
            }

            string GetCanUpgradeShelterWithNewItemMessage()
            {
                var recived = LocalizationModel.GetString(LocalizationKeyID.Quest_ReceivedQuestItem);
                var questItemName = QuestsModel.QuestItemData != null ? LocalizationModel.GetString(QuestsModel.QuestItemData.LocalizationKeyID) : "quset item";
                var canUpgrade = LocalizationModel.GetString(LocalizationKeyID.Quest_UpgradeAwaliable);

                var recivedItem = recived.Replace("#", questItemName);
                return $"{questItemName} {recived.ToLower()}. {canUpgrade}";
            }
        }

        private void ShowMessageForSomeTime(string message, Sprite icon, float time)
        {
            QuestNotificationsModel.Show(message,icon);
            coroutineIndex = CoroutineModel.InitCoroutine(DoAfterSeconds(time,() => {
                QuestNotificationsModel.Hide();
                coroutineIndex = -1;
            }));
        }

        private void OnCollectMedallions()
        {
            var icon = MedallionsModel.Icon;
            InGameNotificationData notificationData = new InGameNotificationData(GetMessage,icon);

            NotificationsModel.SendNotification(notificationData,highPriority: true);

            string GetMessage() => $"{Recived()} {MedallionsModel.Collected}/{MedallionsModel.Total} {Medalions()}";
            string Recived() => LocalizationModel.GetString(LocalizationKeyID.Quest_ReceivedQuestItem);
            string Medalions() => LocalizationModel.GetString(LocalizationKeyID.Collectable_Medallions);
        }

        private void OnNotificationDataAdded()
        {
            UpdateObjectiveView();
        }

        private bool TryShowInGameNotifications()
        {
            bool noNotifications = NotificationsModel.notificationDatas.Count == 0;

            if(noNotifications) return false;

            if(ShelterAttackModeModel.AttackModeActive) return false;

            if(CinematicModel.CinematicStarted) return false;
            
            if(view == null || needToResetViews)
            {
                view = ViewsSystem.Show(ViewConfigID.QuestObjective);
                needToResetViews = false;
            }

            var notification = NotificationsModel.notificationDatas.Dequeue();

            ShowMessageForSomeTime(notification.message.Invoke() ,notification.icon,messageShowTime);
            extraCoroutineIndex = CoroutineModel.InitCoroutine(DoAfterSeconds(messageShowTime + showDelayAfterSideQuest,() => {
                // ResetViews();
                ResetViews();
                extraCoroutineIndex = -1;
            }));

            return true;
        }

        private void OnSideQuestUpdated(QuestData questData)
        {
            if(SideQuestsModel.StageShowPopup(questData))
            {
                Sprite icon = SideQuestsModel.StageIcon(questData);

                NotificationsModel.SendNotification(new InGameNotificationData(GetMessage,icon),highPriority:true);

                string GetMessage() => SideQuestsModel.StageDescription(questData);
            }
        }


        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator DoAfterSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}