using UnityEngine;

namespace Encounters
{
    public static class EnemyConfigFactory
    {
        public enum EnemyActivityConfig
        {
            fightingEnemies,
            skeleton_kamikadze,
            archer,
            crabBoss,
            crabChest,
            boar_passive,
            skeletonLite_scared,
            skeletonWarrior_scared,
            skeletonKamikadze_attacking,
            skeletonArcher_attacking,
            
        }

        public static EnemyConfig GetConfig(EnemyActivityConfig id)
        {
            EnemyConfig fightingEnemies = new EnemyConfig(EnemyID.skeleton_warrior, false, false, false, true);
            EnemyConfig skeleton_kamikadze = new EnemyConfig(EnemyID.skeleton_kamikadze);
            EnemyConfig archer = new EnemyConfig(EnemyID.skeleton_archer);
            EnemyConfig crabBoss = new EnemyConfig(EnemyID.crab_boss, false,true,false,true);
            EnemyConfig crabChest = new EnemyConfig(EnemyID.crab_normal_chestbody, false, false, true, false);
            EnemyConfig boar_passive = new EnemyConfig(EnemyID.boar, false, false, false, true);
            EnemyConfig skeletonLite_scared = new EnemyConfig(EnemyID.skeleton_lite, false, false, true, false);
            EnemyConfig skeletonWarrior_scared = new EnemyConfig(EnemyID.skeleton_warrior, false, false, true, false);
            EnemyConfig skeletonKamikadze_attacking = new EnemyConfig(EnemyID.skeleton_kamikadze, true, true, false, true);
            EnemyConfig skeletonArcher_attacking = new EnemyConfig(EnemyID.skeleton_archer, true, true, false, true);

            switch (id)
            {
                case EnemyActivityConfig.fightingEnemies:
                    return fightingEnemies;
                case EnemyActivityConfig.skeleton_kamikadze:
                    return skeleton_kamikadze;
                case EnemyActivityConfig.archer:
                    return archer;
                case EnemyActivityConfig.crabBoss:
                    return crabBoss;
                case EnemyActivityConfig.crabChest:
                    return crabChest;
                case EnemyActivityConfig.boar_passive:
                    return boar_passive;
                case EnemyActivityConfig.skeletonLite_scared:
                    return skeletonLite_scared;
                case EnemyActivityConfig.skeletonWarrior_scared:
                    return skeletonWarrior_scared;
                case EnemyActivityConfig.skeletonKamikadze_attacking:
                    return skeletonKamikadze_attacking;
                case EnemyActivityConfig.skeletonArcher_attacking:
                    return skeletonArcher_attacking;
                default:
                    Debug.LogError("Not supported");
                    return null;
            }
        }
    }
}