using System;
using System.Collections.Generic;
using System.Linq;
using ActivityLog.Data;
using Chances;
using Core.Storage;
using Encounters;
using Encounters.Analytics;
using Game.LocalNotifications;
using Game.Models.Encounters;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public class EncountersModel : InitableModel<EncountersModel.Data>, IActivityLogEnterencesModel // used initableModel because of OnInited Method, Data not used
    {
        [System.Serializable]
        private class Config
        {
            [SerializeField] private EncounterBaseModel _model;
            [SerializeField] private EncounterConfig _config;
            [SerializeField] private EncounterConfig _debugConfig;
            [SerializeField] private bool _useDebugConfig;
            [SerializeField] private bool _init = true;

            public EncounterBaseModel model => _model;
            public EncounterConfig config => _useDebugConfig && Debug.isDebugBuild ? _debugConfig : _config;
            public bool init => _init;
        }

        [Serializable]
        private class SpecialConfig
        {
            [SerializeField] private SpecialEncounterModel _model;
            [SerializeField] private EncounterConfig _config;
            [SerializeField] private EncounterConfig _debugConfig;
            [SerializeField] private bool _useDebugConfig;
            [SerializeField] private LootGroupID _lootGroup;
            [SerializeField] private Sprite _icon;
            [SerializeField] private bool _init = true;

            public SpecialEncounterModel model => _model;
            public EncounterConfig config => _useDebugConfig ? _debugConfig : _config;
            public LootGroupID lootGroup => _lootGroup;
            public Sprite icon => _icon;
            public bool init => _init;
        }


        [Serializable]
        public class Data : DataBase { }

        #region Data
#pragma warning disable 0649
        [SerializeField] private Data _data;


        [Header("Locations encounters")]
        [SerializeField] private LocationEncountersConfig _locationEncountersConfig;
        
        [Space]
        [SerializeField] private Config location_Encounter_1;
        [SerializeField] private Config location_Encounter_2;
        [SerializeField] private Config location_group_1;

        [Header("Time encounters")]
        [SerializeField] private TimeEncounterConfig time_EncountersConfig;
        
        [Space]
        [SerializeField] private Config time_progressiveEncounter_1;


        [Header("Special encounters")]
        [SerializeField] private SpecialConfig special_bossEncounter_1;
        [SerializeField] private Sprite _specialEncounterIcon;
        [SerializeField] private SpecialConfig special_chest_1;
        [SerializeField] private SpecialConfig special_chest_2;
        [SerializeField] private SpecialConfig special_movingGroup_1;

        #if UNITY_EDITOR
        [Header("Extra")]
        [SerializeField] private LootGroupID _testLootDropID;
        [Button] void TestDrop() => new RewardDrop(_testLootDropID).TestDrop();
        #endif

#pragma warning restore 0649
        #endregion
        protected override Data DataBase => _data;

        #region Dependencies
        private BoarWaypontsProvider BoarWaypontsProvider {get => FindObjectOfType<BoarWaypontsProvider>();} // TODO: redo
        private LocalizationModel LocalizationModel {get => ModelsSystem.Instance._localizationModel;}
            
        #endregion
        protected override void OnInited()
        {
            activities = null;
            // TODO: handle that all models are inited
            InitStorage();
            InitEncounters();
        }

        public event Action<ISpawnPointProvider> OnPlayerEnterZone;

        public List<IEncounter> LocationEncounters {get; private set;}
        public List<IEncounter> TimeEncounters {get; private set;}
        public List<ISpecialEncounter> SpecialEncounters {get; private set;}

        public IProgressiveSmartDice locationEncounterDice;
        public IProgressiveSmartDice timeEncounterDice;

        public float LocationEncounterBaseChance => _locationEncountersConfig.baseChance;
        public float LocationEncounterChanceStep => _locationEncountersConfig.chanceStep;

        public float TimeEncounterBaseChance => time_EncountersConfig.baseChance;
        public float TimeEncounterChanceStep => time_EncountersConfig.chanceStep;
        public float TimeEncounterUpdateTime => time_EncountersConfig.firstDiceRollTime;
        public float TimeEncounterFirstUpdateTime => time_EncountersConfig.diceRerollTime;

        public void PlayerEnterZone(ISpawnPointProvider spawnPointProvider)
        {
            OnPlayerEnterZone?.Invoke(spawnPointProvider);
        }

        public void AvaliableActivitiesCountUpdated() => OnActivitiesCountChanged?.Invoke();

        // TODO: handle that all models are inited
        private void InitStorage()
        {
            // location encounters
            location_Encounter_1.model.Init();
            location_Encounter_2.model.Init();
            location_group_1.model.Init();

            // time encounters
            time_progressiveEncounter_1.model.Init();

            // special encounters
            special_bossEncounter_1.model.Init();
            special_chest_1.model.Init();
            special_chest_2.model.Init();
        }

        private void InitEncounters()
        {
            LocationEncounters = GetLocationEncounters();
            TimeEncounters = GetTimeEncounters();
            SpecialEncounters = GetSpesialEncounter();
        }

        // development
        public void Reset()
        {
            // TODO: handle that all models are inited
            // location encounters
            location_Encounter_1.model.Reset();
            location_Encounter_2.model.Reset();
            location_group_1.model.Reset();

            // time encounters
            time_progressiveEncounter_1.model.Reset();

            // special encounters
            special_bossEncounter_1.model.Reset();
            special_chest_1.model.Reset();
            special_chest_2.model.Reset();

            InitEncounters();
        }

        private List<IEncounter> GetLocationEncounters()
        {
            var encounter_1_activityConfig = EnemyConfigFactory.EnemyActivityConfig.skeleton_kamikadze;
            var encounter_2_activityConfig = EnemyConfigFactory.EnemyActivityConfig.archer;

            var fightingSkeleton = EnemyConfigFactory.EnemyActivityConfig.fightingEnemies;

            bool canDespawnOnFarAway = true;

            var answer = new List<IEncounter>()
            {
                Factory.GetSimpleEnemyEncounter(location_Encounter_1.init,
                                                location_Encounter_1.config,
                                                location_Encounter_1.model,
                                                new SimpleEnemyActivityFactory(encounter_1_activityConfig,canDespawnOnFarAway),
                                                new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_location_1,encounter_1_activityConfig.ToString(),EncounterType.location))),

                Factory.GetSimpleEnemyEncounter(location_Encounter_2.init,
                                                location_Encounter_2.config,
                                                location_Encounter_2.model,
                                                new SimpleEnemyActivityFactory(encounter_2_activityConfig,canDespawnOnFarAway),
                                                new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_location_2,encounter_2_activityConfig.ToString(),EncounterType.location))),

                Factory.GetSimpleEnemyEncounter(location_group_1.init, location_group_1.config, location_group_1.model,
                                                    new FightingEnemiesActivityFactory(new List<EnemyConfigFactory.EnemyActivityConfig>(){fightingSkeleton,fightingSkeleton}),
                                                    new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_location_3,"fightingSkeletons x2",EncounterType.location))),
            };

            answer = answer.Where(x => x != null).ToList();

            return answer;
        }


        private List<IEncounter> GetTimeEncounters()
        {
            var configs = new List<EnemyConfigFactory.EnemyActivityConfig>()
            {
                EnemyConfigFactory.EnemyActivityConfig.skeletonLite_scared,
                EnemyConfigFactory.EnemyActivityConfig.skeletonWarrior_scared,
                EnemyConfigFactory.EnemyActivityConfig.skeletonKamikadze_attacking,
                EnemyConfigFactory.EnemyActivityConfig.skeletonArcher_attacking
            };

            var sceneActivityFactory = new ProgressableEnemyActivityFactory(StretchConfigs(configs,5),time_progressiveEncounter_1.model,EncounterID.encounter_time_1,EncounterType.time);
            var answer = new List<IEncounter>()
            {
                Factory.GetSimpleEnemyEncounter(time_progressiveEncounter_1.init, time_progressiveEncounter_1.config,
                                                time_progressiveEncounter_1.model,
                                                sceneActivityFactory,
                                                sceneActivityFactory),
            };

            answer = answer.Where(x => x != null).ToList();


            return answer;
        }

        private List<ISpecialEncounter> GetSpesialEncounter()
        {
            var boar_passive = EnemyConfigFactory.EnemyActivityConfig.boar_passive;

            bool canDespawnOnFarAway = false;

            var answer = new List<ISpecialEncounter>()
            {
                Factory.GetSpecialEnemyEncounter(special_bossEncounter_1.init,
                                                 special_bossEncounter_1.config,
                                                 special_bossEncounter_1.model,
                                                 new SimpleEnemyActivityFactory(EnemyConfigFactory.EnemyActivityConfig.crabBoss,canDespawnOnFarAway),
                                                 new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_special_1,"crabBoss",EncounterType.special)),
                                                 () => LocalizationModel.GetString(LocalizationKeyID.Activity_SpecialEncounter_crabBoss_1),
                                                 GetActivityCompleatedMessage,
                                                 LocalNotificationID.SpecialEncounter_crab,
                                                 special_bossEncounter_1.icon,
                                                 special_bossEncounter_1.lootGroup),
                Factory.GetSpecialEnemyEncounter(special_chest_1.init,
                                                 special_chest_1.config,
                                                 special_chest_1.model,
                                                 new SimpleEnemyActivityFactory(EnemyConfigFactory.EnemyActivityConfig.crabChest,canDespawnOnFarAway),
                                                 new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_special_2,"chest_crab_v1",EncounterType.special)),
                                                 () => LocalizationModel.GetString(LocalizationKeyID.Activity_SpecialEncounter_chestCrab_1),
                                                 GetActivityCompleatedMessage,
                                                 LocalNotificationID.SpecialEncounter_chestCrab,
                                                 special_chest_1.icon,
                                                 special_chest_1.lootGroup),
                Factory.GetSpecialEnemyEncounter(special_chest_2.init,
                                                 special_chest_2.config,
                                                 special_chest_2.model,
                                                 new SimpleEnemyActivityFactory(EnemyConfigFactory.EnemyActivityConfig.crabChest,canDespawnOnFarAway),
                                                 new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_special_3,"chest_crab_v2",EncounterType.special)),
                                                 () => LocalizationModel.GetString(LocalizationKeyID.Activity_SpecialEncounter_chestCrab_1),
                                                 GetActivityCompleatedMessage,
                                                 LocalNotificationID.SpecialEncounter_chestCrab,
                                                 special_chest_2.icon,
                                                 special_chest_2.lootGroup),
                Factory.GetSpecialEnemyEncounter(special_movingGroup_1.init,
                                                 special_movingGroup_1.config,
                                                 special_movingGroup_1.model,
                                                 new EnemiesGroupActivityFactory(new List<EnemyConfigFactory.EnemyActivityConfig>{ boar_passive,boar_passive},BoarWaypontsProvider.WayPoints),
                                                 new SimpleAnaliticsDataProvider(new EncounterAnaliticsData(EncounterID.encounter_special_4,"boars(x2)walking",EncounterType.special))),
            };

            answer = answer.Where(x => x != null).ToList();

            return answer;

            string GetActivityCompleatedMessage() => LocalizationModel.GetString(LocalizationKeyID.Activity_Completed);
        }


        private List<EnemyConfigFactory.EnemyActivityConfig> StretchConfigs(List<EnemyConfigFactory.EnemyActivityConfig> configs, int count)
        {
            var answer = new List<EnemyConfigFactory.EnemyActivityConfig>();

            foreach (var config in configs)
            {
                for (int i = 0; i < count; i++)
                {
                    answer.Add(config);
                }
            }

            return answer;
        }

        #region Push notifications
        private IPushNotificationProducer[] activities;
        public IPushNotificationProducer[] GetPushNotificationProducers()
        {
            if(activities == null)
            {
                activities = SpecialEncounters.ToArray() as IPushNotificationProducer[];
            }

            return activities;
        }
            
        #endregion

        #region IActivityLogEnterencesModel

        public event Action OnActivitiesCountChanged;

        public List<ActivityLogEnterenceData> GetActivitiesEnterences()
        {
            var answer = new List<ActivityLogEnterenceData>();

            foreach (var encounter in SpecialEncounters)
            {
                if(!encounter.IsActivated) continue;

                if(encounter.TryGetActivityLogEnterence(out var activity))
                {
                    answer.Add(activity);
                }
            }

            return answer;
        }
            
        #endregion

        #region Analytics
        public event Action<EncounterInitEvent> OnEncounterInit;
        public event Action<EncounterCompleateEvent> OnEncounterCompleate;
        public event Action<EncounterUnloadEvent> OnEncounterUnload;

        public void SendEncounterInitEvent(EncounterInitEvent initEvent)
        {
            Debug.Log($"Init Event: {initEvent.ToString()}");
            OnEncounterInit?.Invoke(initEvent);
        }

        public void SendEncounterCompleateEvent(EncounterCompleateEvent compleateEvent)
        {
            Debug.Log($"Init Event: {compleateEvent.ToString()}");
            OnEncounterCompleate?.Invoke(compleateEvent);
        }

        public void SendEncounterUnloadEvent(EncounterUnloadEvent unloadEvent)
        {
            Debug.Log($"Init Event: {unloadEvent.ToString()}");
            OnEncounterUnload?.Invoke(unloadEvent);
        }
            
        #endregion


        public IEncounter GetRandomAvaliableEncounter(List<IEncounter> encounters)
        {
            float chancesSum = 0;

            foreach (var encounter in encounters)
            {
                if (!encounter.CanOccure) continue;
                if (encounter.IsActive) continue;

                chancesSum += encounter.chanceWeight;
            }

            var randomValue = UnityEngine.Random.Range(0, chancesSum);

            float temp = 0;

            foreach (var encounter in encounters)
            {
                if (!encounter.CanOccure) continue;
                if (encounter.IsActive) continue;

                temp += encounter.chanceWeight;

                if (temp >= randomValue)
                    return encounter;
            }

            return null;
        }

        public void DeactivateEncounters(IEncounter[] encounters)
        {
            for (int i = 0; i < encounters.Count(); i++)
            {
                try
                {
                    encounters[i].DeInit();
                    Debug.Log($"Despawned {i}");
                }
                catch (System.Exception e)
                {
                    Debug.LogException(new System.Exception($"Error on deactivate in encounter {i}: {e.ToString()}"));
                }
            }
        }
    }
}