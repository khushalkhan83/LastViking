using System;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Encounters
{
    public abstract class EncounterBase : IEncounter
    {
        #region Dependencies
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        public TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
        public QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        #endregion

        private const long k_ticksPerSecond = 8640000;

        private List<Action> deactivateActions = new List<Action>();

        #region IEncounter

        public abstract bool CanOccure {get;}
        public abstract float chanceWeight {get;}
        public bool IsActive {get; private set;}
        public bool isCooldown => cooldown > 0;
        public float cooldown
        {
            get
            {
                var timeFromLastSpawn = GameTimeModel.RealTimeNowTick - model.LastSpawnTimeTicks;
                var answer = resetTimeSeconds * k_ticksPerSecond - timeFromLastSpawn;
                if(answer < 0)
                {
                    answer = 0;
                }

                return answer / k_ticksPerSecond;
            }
        }
        public bool IsUnlocked => QuestsModel.GetChapter() >= neededStoryChapter;
        public void Init(Vector3 spawnPoint)
        {
            IsActive = true;
        }
        public void AddActionOnDeactivate(Action action) => deactivateActions.Add(action);

        public virtual void DeInit()
        {
            if(IsActive)
                DeActivate();
            else
            {
                Debug.LogError("Ignored deactivate actions (no content on scene inited)");
            }
        }

        #endregion

        protected abstract IEncounterStorage model {get;}
        protected abstract float resetTimeSeconds {get;}
        protected abstract int neededStoryChapter {get;}
        public void Compleate()
        {
            IsActive = false;
            SaveLastSpawnTime();
            model.CompletionCounter++;
            "Compleated encounter".Log();
            FireDeactivateActionsAndClean();
        }
        public void DeActivate()
        {
            IsActive = false;
            SaveLastSpawnTime();
            "Deactivate encounter".Log();

            FireDeactivateActionsAndClean();
        }

        public void Unload()
        {
            IsActive = false;
            "Unload encounter".Log();

            FireDeactivateActionsAndClean();
        }

        private void SaveLastSpawnTime() => model.LastSpawnTimeTicks = GameTimeModel.RealTimeNowTick;
        private void FireDeactivateActionsAndClean()
        {
            foreach (var action in deactivateActions)
            {
                try
                {
                    action?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(new System.Exception($"Error in deactivate actions {e.ToString()}"));
                }
            }

            deactivateActions.Clear();
        }
    }
}