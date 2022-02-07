using System.Collections.Generic;
using Core;
using Game.Models;
using UnityEngine;
using Encounters;
using Chances;
using Core.Controllers;

namespace Game.Controllers
{
    public class LocationEncountersController : IController, ILocationEncountersController
    {
        public EncountersModel EncountersModel { get; } = GameObject.FindObjectOfType<EncountersModel>(); // TODO: use injeect

        // [Inject] public EncountersModel EncountersModel { get; private set; }
        [Inject] public EncountersViewModel ViewModel { get; private set; }

        private List<IEncounter> LocationEncounters {get => EncountersModel.LocationEncounters;}

        private ISpawnPointProvider lastSpawnPointProvider; // TODO: move to namespace
        private IProgressiveSmartDice locationEncounterDice {get => EncountersModel.locationEncounterDice; set => EncountersModel.locationEncounterDice = value;}


        public void Enable()
        {
            locationEncounterDice = new ProgressiveSmartDice(EncountersModel.LocationEncounterBaseChance, EncountersModel.LocationEncounterChanceStep);
            EncountersModel.OnPlayerEnterZone += OnNewZoneLoaded;
        }

        public void Start() { }    

        public void Disable()
        {
            EncountersModel.OnPlayerEnterZone -= OnNewZoneLoaded;
            EncountersModel.DeactivateEncounters(LocationEncounters.ToArray());
        }


        // TODO: fix zone entered trigger when game init
        private void OnNewZoneLoaded(ISpawnPointProvider spawnPointProvider)
        {
            if (spawnPointProvider == null)
            {
                ViewModel.PresentEventMessage("Null spawn point", false); // TODO: add error log here 
                return;
            }

            var spawnPointError = !spawnPointProvider.TryGetValidSpawnPoint(out var spawnPoint);

            if(spawnPointError)
            {
                ViewModel.PresentEventMessage("Can`t get spawn point", false); // TODO: add error log here 
                return;
            }

            if(spawnPointProvider == lastSpawnPointProvider)
            {
                ViewModel.PresentEventMessage("Can`t use same location twice",false);
                return;
            }

            lastSpawnPointProvider = spawnPointProvider;

            var encounter = GetLocationEncounter();

            if (encounter == null)
            {
                ViewModel.PresentEventMessage("No encounter avaliable", false);
                return;
            }

            var rollResult = locationEncounterDice.GetRollResult();

            ViewModel.PresentEventMessage("Location dice roll", rollResult);

            if (rollResult == false)
            {
                ViewModel.Refresh();
                return;
            }

            try
            {
                encounter.Init(spawnPoint);
            }
            catch (System.Exception e)
            {
                ViewModel.PresentEventMessage("Location encounter init error", false);
                Debug.LogException(new System.Exception($"Location encounter init error: {e.ToString()}"));
            }


            ViewModel.Refresh();
        }


        private IEncounter GetLocationEncounter()
        {
            var randomEncounter = EncountersModel.GetRandomAvaliableEncounter(LocationEncounters);
            return randomEncounter;
        }
    }
}