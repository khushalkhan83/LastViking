using System;
using Encounters.Analytics;
using Game.Models;
using UnityEngine;

namespace Encounters
{
    // TODO: add unsubscriptions
    public class EnemyEncounter : IEncounter
    {
        #region Dependencies
        private EncountersModel EncountersModel => ModelsSystem.Instance._encountersModel;    
        #endregion

        private readonly EncounterBase encounter;
        private readonly ISceneActivityFactory sceneActivityFactory;
        private readonly IEncounterAnaliticsDataProvider analiticsDataProvider;

        private ISceneActivity sceneActivity;

        public EnemyEncounter(EncounterBase encounter, ISceneActivityFactory sceneActivityFactory, IEncounterAnaliticsDataProvider analiticsDataProvider)
        {
            this.encounter = encounter;
            this.sceneActivityFactory = sceneActivityFactory;
            this.analiticsDataProvider = analiticsDataProvider;
        }

        #region IEncounter
        public bool CanOccure => encounter.CanOccure;
        public float chanceWeight => encounter.chanceWeight;
        public float cooldown => encounter.cooldown;
        public bool IsActive => encounter.IsActive;
        public bool isCooldown => encounter.isCooldown;
        public bool IsUnlocked => encounter.IsUnlocked;
        public void Init(Vector3 spawnPoint)
        {
            sceneActivity = sceneActivityFactory.GetActivity();
            sceneActivity.Spawn(spawnPoint);
            sceneActivity.OnCompleated += OnCompleatedActivity;
            sceneActivity.OnFarAwayDespawned += OnFarAwayDespawned;

            encounter.Init(spawnPoint);

            EncountersModel.SendEncounterInitEvent(new EncounterInitEvent(analiticsDataProvider.GetAnaliticsData()));
        }

        public void AddActionOnDeactivate(Action action)
        {
            encounter.AddActionOnDeactivate(action);
        }

        public void DeInit()
        {
            if(!encounter.IsActive) return;
            if(sceneActivity == null) return;
            
            encounter.DeInit();
            sceneActivity.Despawn();

            EncountersModel.SendEncounterUnloadEvent(new EncounterUnloadEvent(analiticsDataProvider.GetAnaliticsData(),EncounterUnloadReason.deactivate));
        }

        public void Compleate()
        {
            encounter.Compleate();

            EncountersModel.SendEncounterCompleateEvent(new EncounterCompleateEvent(analiticsDataProvider.GetAnaliticsData()));
        }

        #endregion

        private void OnCompleatedActivity() => Compleate();

        private void OnFarAwayDespawned()
        {
            encounter.Unload();
            sceneActivity.Despawn();

            EncountersModel.SendEncounterUnloadEvent(new EncounterUnloadEvent(analiticsDataProvider.GetAnaliticsData(),EncounterUnloadReason.farAwayDespawn));
        }
    }
}