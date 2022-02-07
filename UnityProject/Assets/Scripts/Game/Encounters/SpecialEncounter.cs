using System;
using ActivityLog.Data;
using Encounters.Analytics;
using Game.InGameNotifications;
using Game.Models;
using Game.Models.Encounters;
using UnityEngine;

namespace Encounters
{
    public class SpecialEncounter : ISpecialEncounter
    {
        private readonly IInGameNotification winNotification;
        private readonly ActivityLogEnterenceData activitiLogData;
        private readonly SpecialEncounterModel specialEncounterModel;
        private readonly IInGameNotification startEventNotification;
        private readonly IRewardDrop rewardDrop;
        private readonly IEncounterAnaliticsDataProvider analiticsDataProvider;
        private readonly bool initedInTutorial;

        #region Dependencies
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
        private EncountersModel EncountersModel => ModelsSystem.Instance._encountersModel;
            
        #endregion

        public SpecialEncounter(EncounterBase eventBase, ISceneActivityFactory sceneActivityFactory,
                                SpecialEncounterModel encounterModel,
                                IInGameNotification startEventNotification,
                                IInGameNotification winNotification,
                                ActivityLogEnterenceData activitiLogData,
                                IRewardDrop rewardDrop,
                                IEncounterAnaliticsDataProvider analiticsDataProvider, LocalNotificationID? notificationID)
        {
            this.eventBase = eventBase;
            this.sceneActivityFactory = sceneActivityFactory;
            this.specialEncounterModel = encounterModel;
            this.startEventNotification = startEventNotification;
            this.winNotification = winNotification;
            this.activitiLogData = activitiLogData;
            this.rewardDrop = rewardDrop;
            this.analiticsDataProvider = analiticsDataProvider;
            this.LocalNotificationID = notificationID;
            initedInTutorial = !TutorialModel.IsComplete;
        }

        private readonly EncounterBase eventBase;
        private readonly ISceneActivityFactory sceneActivityFactory;
        private ISceneActivity sceneActivity;

        #region ISpecialEncounter
        public bool IsActivated {get => specialEncounterModel.IsActivated ; private set => specialEncounterModel.IsActivated = value;}
        public void SaveSpawnerIndex(int index) => specialEncounterModel.SpawnPointIndex = index;
        public bool TryGetSpawnerIndex(out int index)
        {
            index = specialEncounterModel.SpawnPointIndex;

            return specialEncounterModel.IsActivated;
        }
            
        #endregion

        #region IPushNotificationProducer
        public bool canSendPushNotifications => true;
        public LocalNotificationID? LocalNotificationID { get; }
        public float cooldownSeconds => cooldown;
        public bool isLocked => !IsUnlocked;
        #endregion

        #region IActivityLogEnterenceProducer

        public bool TryGetActivityLogEnterence(out ActivityLogEnterenceData activityLog)
        {
            activityLog = activitiLogData;
            return activityLog != null;
        }
            
        #endregion

        #region IEncounter
        public bool CanOccure => eventBase.CanOccure;
        public float chanceWeight => eventBase.chanceWeight;
        public float cooldown => eventBase.cooldown;
        public bool IsActive => eventBase.IsActive;
        public bool isCooldown => eventBase.isCooldown;
        public bool IsUnlocked => eventBase.IsUnlocked;
        public void Compleate() => eventBase.Compleate();
        public void AddActionOnDeactivate(Action action) => eventBase.AddActionOnDeactivate(action);
        public void Init(Vector3 spawnPoint)
        {
            if (!IsActivated)
            {
                IsActivated = true;
                EncountersModel.SendEncounterInitEvent(new EncounterInitEvent(analiticsDataProvider.GetAnaliticsData()));
            }

            if(initedInTutorial) return;

            eventBase.Init(spawnPoint);

            startEventNotification?.Show();
            sceneActivity = sceneActivityFactory.GetActivity();
            sceneActivity.Spawn(spawnPoint);
            sceneActivity.OnCompleated += EncounterCompleateHandler;
            sceneActivity.OnFarAwayDespawned += OnFarAwayDespawned;
        }

        public void DeInit()
        {
            if(!eventBase.IsActive) return;
            if(sceneActivity == null) return;

            sceneActivity.Despawn();

            eventBase.Unload();
        }

        #endregion

        private void EncounterCompleateHandler()
        {
            if (specialEncounterModel.IsActivated)
            {
                specialEncounterModel.IsActivated = false;
                EncountersModel.SendEncounterCompleateEvent(new EncounterCompleateEvent(analiticsDataProvider.GetAnaliticsData()));
            }

            winNotification?.Show();
            rewardDrop?.Drop(sceneActivity.position);

            eventBase.Compleate();
        }

        private void OnFarAwayDespawned()
        {
            eventBase.Unload();
        }
    }
}