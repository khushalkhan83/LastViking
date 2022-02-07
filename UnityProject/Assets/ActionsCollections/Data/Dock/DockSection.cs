using System.Collections.Generic;
using ActionsCollections;
using CustomeEditorTools;
using Game.Models;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static DockSection Dock {get;} = new DockSection();}
    public class DockSection : SectionBase
    {
        public override string SectionName => "Dock";

        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new DockActionUpgrade("Upgrade"));
            
            for (int i = 0; i < 11; i++)
            {
                answer.Add(new DockActionSetShipLevel($"level {i}",i));
            }           

            return answer;
        }
    }
}