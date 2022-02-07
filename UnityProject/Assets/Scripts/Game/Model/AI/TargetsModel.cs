using System;
using System.Collections.Generic;
using Game.AI;
using Game.VillageBuilding;
using UnityEngine;

namespace Game.Models.AI
{
    public class TargetsModel : MonoBehaviour
    {
        public Func<List<Target>> OnTownHallPartTargets;

        public List<Target> TownHallPartTargets => OnTownHallPartTargets?.Invoke();
        public List<Target> DefenceTargets;
        public List<DestroyableBuilding> Towers;
        public List<DestroyableBuilding> Walls;

        public Func<bool> GetHasTargetsForAttack;

        public bool HasTargetsForAttack => GetHasTargetsForAttack.Invoke();

    }
}
