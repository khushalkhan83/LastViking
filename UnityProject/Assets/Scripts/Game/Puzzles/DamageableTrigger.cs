using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;


namespace Game.Puzzles
{
    public abstract class DamageableTrigger : TriggerBase, IDamageable
    {
        public void Damage(float value, GameObject from = null)
        {
            UseTrigger();
        }
    }
}
