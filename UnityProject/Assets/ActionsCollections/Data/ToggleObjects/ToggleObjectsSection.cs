using System.Collections.Generic;
using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static ToggleObjectsSection Toggle {get;} = new ToggleObjectsSection();}
    public class ToggleObjectsSection : SectionBase
    {
        public override string SectionName => "ToggleObjects";
        public override List<ActionBase> Actions
        {
            get => new List<ActionBase>()
                {
                    new ToggleObjectAction("GLight"),
                    new ToggleObjectAction("WorldObjects"),

                    new ToggleObjectAction("GEnvironment"), //
                    new ToggleObjectAction("Spawners"),
                    new ToggleObjectAction("Terrain_Island"), //
                    new ToggleObjectAction("Trees"),
                    new ToggleObjectAction("GRocks"),
                    new ToggleObjectAction("Other"),
                    new ToggleObjectAction("Grass"),
                    new ToggleObjectAction("Butterflies"),

                    new ToggleObjectAction("Monuments"),
                    new ToggleObjectAction("Loot"),
                    new ToggleObjectAction("Debris"),
                    new ToggleObjectAction("FishingPlaces"),
                    new ToggleObjectAction("TrasurePlaces"),
                    new ToggleObjectAction("Transitions_Island"),

                };
        }
    }

}