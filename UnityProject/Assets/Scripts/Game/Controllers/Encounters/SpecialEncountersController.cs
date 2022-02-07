using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using Encounters;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class SpecialEncountersController : ISpecialEncountersController, IController
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        // [Inject] public EncountersModel EncountersModel { get; private set; }
        private EncountersModel EncountersModel { get; } = GameObject.FindObjectOfType<EncountersModel>(); // TODO: use injeect
        [Inject] public EncountersViewModel ViewModel { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject(true)] public SpecialEncounterSpawnPointProvider SpecialEncounterSoketProvider { get; private set; }

        private List<ISpecialEncounter> SpecialEncounters {get => EncountersModel.SpecialEncounters;}

        void IController.Enable() 
        {
            if(TutorialModel.IsComplete)
            {
                Init();
            }
            else
            {
                TutorialModel.OnComplete += DelayedInit;
            }
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            TutorialModel.OnComplete -= DelayedInit;
            QuestsModel.OnActivateStage -= TrySpawnSpecialEvent;

            EncountersModel.DeactivateEncounters(SpecialEncounters.ToArray());
        }

        private void DelayedInit()
        {
            TutorialModel.OnComplete -= DelayedInit;
            Init();
        }

        private void Init()
        {
            QuestsModel.OnActivateStage += TrySpawnSpecialEvent;
            TrySpawnSpecialEvent();
        }

        private void TrySpawnSpecialEvent()
        {
            var activatedEncounters = SpecialEncounters.Where(x => x.IsActivated);
            var notActivatedEncounters = SpecialEncounters.Where(x => x.IsActivated ==false);
            int activatedEncountersCount = activatedEncounters.Count();


            var notInited = TryInitSpecialEncounter(activatedEncounters,false);
            
            var other = notInited.ToList();
            other.AddRange(notActivatedEncounters);

            TryInitSpecialEncounter(other,true);

            int newActiveEncountersCount = SpecialEncounters.Where(x => x.IsActivated).Count();
            if(newActiveEncountersCount != activatedEncountersCount)
            {
                EncountersModel.AvaliableActivitiesCountUpdated();
            }
        }

        private IEnumerable<ISpecialEncounter> TryInitSpecialEncounter(IEnumerable<ISpecialEncounter> specialEncounters, bool useNewSoket)
        {
            var notInited = new List<ISpecialEncounter>();
            foreach (var encounter in specialEncounters)
            {
                if (encounter.CanOccure && !encounter.IsActive)
                {
                    bool error = !TryGetSpawnPointProvider(encounter,out ISpawnPointProvider spawnPointProvider, out int spawnPointProviderIndex,useNewSoket);

                    if(error)
                    {
                        ViewModel.PresentEventMessage("Can`t get valid spawn point provider", false);
                        notInited.Add(encounter);
                        continue;
                    }

                    bool spawnPointError = !spawnPointProvider.TryGetValidSpawnPoint(out var spawnPoint);

                    if(spawnPointError)
                    {
                        ViewModel.PresentEventMessage("Can`t get valid spawn point", false);
                        notInited.Add(encounter);
                        continue;
                    }

                    try
                    {
                        encounter.Init(spawnPoint);
                        encounter.SaveSpawnerIndex(spawnPointProviderIndex);

                        SpecialEncounterSoketProvider.SetBusy(spawnPointProvider);
                        encounter.AddActionOnDeactivate(() =>
                        {
                            SpecialEncounterSoketProvider.UnSetBusy(spawnPointProvider);
                            EncountersModel.AvaliableActivitiesCountUpdated();
                        });
                    }
                    catch (System.Exception e)
                    {
                        ViewModel.PresentEventMessage("Error on special event init", false);
                        Debug.LogException(new System.Exception($"SpecialEncountersError: {e.ToString()}"));
                    }
                }
            }

            ViewModel.Refresh();

            return notInited;
        }

        private bool TryGetSpawnPointProvider(ISpecialEncounter encounter, out ISpawnPointProvider spawnPointProvider, out int spawnPointProviderIndex, bool useNewSoket = false)
        {
            bool result = false;

            if(!useNewSoket && encounter.IsActivated && encounter.TryGetSpawnerIndex(out int spawnerIndex))
            {
                result = SpecialEncounterSoketProvider.TryGetSoketByIndex(spawnerIndex, out spawnPointProvider);

                spawnPointProviderIndex = spawnerIndex;
                return result;
            }
            else
            {
                result = SpecialEncounterSoketProvider.TryGetSoket(out spawnPointProvider, out int spawnerIndex2);
                spawnPointProviderIndex = spawnerIndex2;
            }
            return result;
        }
    }
}
