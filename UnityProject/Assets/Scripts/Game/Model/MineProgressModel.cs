using System;
using UnityEngine;

namespace Game.Models
{
    public class MineProgressModel : DungeonProgressModel
    {
        public override EnvironmentSceneID EnvironmentSceneID => EnvironmentSceneID.Mine;
    }
}
