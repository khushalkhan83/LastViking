using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models;

namespace Helpers
{
    public static class DamagableHelper
    {
        public static void ShowDamage(IDamageable damagable, float damage)
        {
            damage.Log("Player damage " + damagable.ToString() + ": ");
        }
    }
}
