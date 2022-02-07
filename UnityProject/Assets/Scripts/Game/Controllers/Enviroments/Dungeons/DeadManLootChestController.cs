using Core;
using Core.Controllers;
using Game.Models;
using System.Collections.Generic;
using UltimateSurvival;
using System.Linq;
using UnityEngine;
using System.Collections;
using System;

namespace Game.Controllers
{
    public interface IDeadManLootChestController { }
    public class DeadManLootChestController : IController, IDeadManLootChestController
    {
        [Inject] public DungeonsProgressModel DungeonsProgressModel { get; private set; }
        [Inject] public DeadManLootChestModel DeadManLootChestModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }
        [Inject] public InjectionModel InjectionModel { get; private set; }


        private int coroutineIndex = -1;
        public void Enable()
        {
            foreach (var dungeonProgressModel in DungeonsProgressModel.DungeonProgressModels)
            {
                dungeonProgressModel.OnPassedLocation += OnProgressResetted;
            }

            WorldObjectsModel.__OnConstructionInitialized += UpdateChestView;
            DeadManLootChestModel.OnUnlockChanged += UpdateChestView;

            InjectionModel.OnSceneDependenciesInjected += _UpdateChestViewAfterProgressablesSpawned;
        }
        public void Disable()
        {
            foreach (var dungeonProgressModel in DungeonsProgressModel.DungeonProgressModels)
            {
                dungeonProgressModel.OnPassedLocation -= OnProgressResetted;
            }

            WorldObjectsModel.__OnConstructionInitialized -= UpdateChestView;
            DeadManLootChestModel.OnUnlockChanged -= UpdateChestView;
            InjectionModel.OnSceneDependenciesInjected -= _UpdateChestViewAfterProgressablesSpawned;

            CoroutineModel.BreakeCoroutine(coroutineIndex);
        }

        public void Start()
        {
            
        }

        private void OnProgressResetted(DungeonProgressModel arg) 
        {
            DeadManLootChestModel.Unlock = true;
            DeadManLootChestModel.NeedResetItems = true;
        }

        private void _UpdateChestViewAfterProgressablesSpawned()
        {
            coroutineIndex = CoroutineModel.InitCoroutine(DoAfterSeconds(2,UpdateChestView));
        }

        
        private void UpdateChestView()
        {
            bool gotReward = !DeadManLootChestModel.Unlock;
            bool hideChest = gotReward;
            
            // can find no chests if wWorldObjectsController not started yet
            bool noValue = !WorldObjectsModel.CreatedWorldObjectsModels.TryGetValue(WorldObjectID.loot_container_epic, out var chests);

            if(noValue || chests == null) return;

            var chest = chests.FirstOrDefault();

            if(chest == null) return;
 
            chest.gameObject.SetActive(!hideChest);
        }

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator DoAfterSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}
