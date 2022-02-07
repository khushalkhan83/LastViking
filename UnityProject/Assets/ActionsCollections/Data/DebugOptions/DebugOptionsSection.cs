using System.Collections.Generic;
using ActionsCollections;
using CustomeEditorTools;
using Game.Models;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static DebugOptionsSection DebugOptions {get;} = new DebugOptionsSection();}
    public class DebugOptionsSection : SectionBase
    {
        public override string SectionName => "DebugOptions";
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private const int SpawnDistance = 3;
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new DebugOptionsActionSwitchDebugObjectivesTime("Switch  Debug objectives time"));
            answer.Add(new DebugOptionsActionSwitchTreasure("Switch  Treasure Debug"));
            answer.Add(new DebugOptionsActionSwitchIgnoreItemsPrice("Switch Upgrade price condition"));
            answer.Add(new DebugOptionsActionFlyCamera("Switch  FlyCam", 1));
            answer.Add(new DebugOptionsActionFlyCameraChangeSpeed("speed +1",1));
            answer.Add(new DebugOptionsActionFlyCameraChangeSpeed("speed -1",-1));

            return answer;
        }

    }


}