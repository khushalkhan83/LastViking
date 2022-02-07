using Game.AI;
using System;
using UnityEngine;

namespace Game.Models
{
    public class StatisticWorldObjectsNodel : MonoBehaviour
    {
        public event Action<TargetID, WorldObjectID> OnKill;

        public void Kill(TargetID targetID, WorldObjectID worldObjectID) => OnKill?.Invoke(targetID, worldObjectID);
    }
}
