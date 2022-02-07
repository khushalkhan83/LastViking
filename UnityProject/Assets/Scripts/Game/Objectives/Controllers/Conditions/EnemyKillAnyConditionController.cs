﻿using Core;
using Game.AI;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using System.Linq;

namespace Game.Objectives.Conditions.Controllers
{
    public class EnemyKillAnyConditionController : BaseConditionController<EnemyKillAnyConditionData, CountConditionDataModel>
    {
        [Inject] public EnemiesModel EnemiesModel { get; private set; }

        protected override void Subscribe()
        {
            EnemiesModel.OnTargetKillEnemy += OnTargetKillEnemyHandler;
        }

        protected override void Unsubscribe()
        {
            EnemiesModel.OnTargetKillEnemy -= OnTargetKillEnemyHandler;
        }

        private void OnTargetKillEnemyHandler(Target target, EnemyID enemyID) => EventProcessing
            (
                data => data.EnemyIDs.Contains(enemyID)
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}
