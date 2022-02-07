using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public static class EnemiesAttackModificators
    {
        public const float PlayerMultiplayer = 1f;
        public const float MainMultiplayer = 1f;
        public const float ObstacleMultiplayer = 3f;
        public const float EnemyMultiplayer = 0f;
       
        public static float GetDamageForTarget(TargetID targetID, float damage)
        {
            switch(targetID)
            {
                case TargetID.Player:
                    return damage * PlayerMultiplayer;
                case TargetID.Main:
                    return damage * MainMultiplayer;
                case TargetID.Obstacle:
                    return damage * ObstacleMultiplayer;
                case TargetID.Enemy:
                    return damage * EnemyMultiplayer;
                default:
                    return damage;
            }
        }
    }
}
