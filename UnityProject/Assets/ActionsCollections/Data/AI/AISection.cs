using System.Collections.Generic;
using ActionsCollections;

namespace DebugActions
{
    public partial class DebugSections {public static AISection AI {get;} = new AISection();}
    public class AISection : SectionBase
    {
        public override string SectionName => "AI";
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private const int SpawnDistance = 3;
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new AIActionResetKrakenHealth("Reset Kraken"));
            
            answer.Add(new AIActionSpawnEnemy("Spawn: skeleton_super_lite",EnemyID.skeleton_super_lite, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: skeleton_lite",EnemyID.skeleton_lite, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: skeleton_warrior",EnemyID.skeleton_warrior, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: zombie_base",EnemyID.zombie_base, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: skeleton_archer",EnemyID.skeleton_archer, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: skeleton_kamikadze",EnemyID.skeleton_kamikadze, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: crab_boss",EnemyID.crab_boss, SpawnDistance));
            answer.Add(new AIActionSpawnEnemy("Spawn: crab_normal",EnemyID.crab_normal, SpawnDistance));

            answer.Add(new AIActionSpawnAnimal("Spawn: Bear(old)",AnimalID.Bear, SpawnDistance));
            answer.Add(new AIActionSpawnAnimal("Spawn: Boar(old)",AnimalID.Boar, SpawnDistance));
            answer.Add(new AIActionSpawnAnimal("Spawn: Chicken(old)",AnimalID.Chicken, SpawnDistance));
            answer.Add(new AIActionSpawnAnimal("Spawn: Wolf(old)",AnimalID.Wolf, SpawnDistance));

            return answer;
        }

    }


}