using System.Collections.Generic;
using Chances;
using Core;
using Core.Controllers;
using Encounters;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using Updaters;

namespace Game.Controllers
{
    public class TimeEncountersController : ITimeEncountersController, IController
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public EncountersViewModel ViewModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        // [Inject] public EncountersModel EncountersModel { get; private set; }
        public EncountersModel EncountersModel { get; } = GameObject.FindObjectOfType<EncountersModel>(); // TODO: use injeect

        private IUpdater timeEncountersUpdater;
        private ISpawnPointProvider ZoneAroundPlayer;
        private IProgressiveSmartDice timeEncounterDice {get => EncountersModel.timeEncounterDice; set => EncountersModel.timeEncounterDice = value;}

        private List<IEncounter> TimeEncounters {get => EncountersModel.TimeEncounters;}

        void IController.Enable() 
        {
            ZoneAroundPlayer = PlayerEventHandler.GetComponent<ISpawnPointProvider>();
            timeEncountersUpdater = new DelayedUpdater(EncountersModel.TimeEncounterUpdateTime,TryInitTimeEncounter,EncountersModel.TimeEncounterFirstUpdateTime);
            timeEncounterDice = new ProgressiveSmartDice(EncountersModel.TimeEncounterBaseChance, EncountersModel.TimeEncounterChanceStep);

            GameUpdateModel.OnUpdate += timeEncountersUpdater.Tick;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= timeEncountersUpdater.Tick;

            EncountersModel.DeactivateEncounters(TimeEncounters.ToArray());
        }

        private void TryInitTimeEncounter()
        {
            ISpawnPointProvider spawnPointProvider = ZoneAroundPlayer;
            if (spawnPointProvider == null)
            {
                ViewModel.PresentEventMessage("No spawn point provider for init soket", false);
                return;
            }

            bool success = spawnPointProvider.TryGetValidSpawnPoint(out Vector3 spawnPoint);
            if (!success)
            {
                ViewModel.PresentEventMessage("No valid spawn point for init soket", false);
                return;
            }
            IEncounter encounter = GetEncounter();

            if (encounter == null)
            {
                ViewModel.PresentEventMessage("No avaliable encounter", false);
                return;
            }

            var rollResult = timeEncounterDice.GetRollResult();

            ViewModel.PresentEventMessage("Location roll result", rollResult);
            if (rollResult == false)
            {
                ViewModel.Refresh();
                return;
            }

            try
            {
                encounter.Init(spawnPoint);
            }
            catch (System.Exception)
            {
                ViewModel.PresentEventMessage("Time encounter init error", false);
                throw;
            }

            ViewModel.Refresh();
        }

        private IEncounter GetEncounter()
        {
            return EncountersModel.GetRandomAvaliableEncounter(TimeEncounters);
        }
    }
}
