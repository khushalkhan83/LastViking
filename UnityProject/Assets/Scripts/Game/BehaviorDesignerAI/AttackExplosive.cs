using BehaviorDesigner.Runtime;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.AI.Components;
using Game.Models;
using System;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public class AttackExplosive : MonoBehaviour
    {
          public event Action<Target> OnKillTarget;

        #region Data
    #pragma warning disable 0649

        [SerializeField] ExplosiveBarrel _explosiveBarrel;

    #pragma warning restore 0649
        #endregion

        //from animation event
        void OnAnimationDamage()
        {
            _explosiveBarrel.Explode();
        }

    }
}
