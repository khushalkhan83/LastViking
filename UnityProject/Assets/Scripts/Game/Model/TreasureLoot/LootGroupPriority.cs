using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Models
{
    [Serializable]
    public class LootGroupPriority
    {
        public LootGroupID id;
        public float dropPriority;
    }
}