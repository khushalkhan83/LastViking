using System.Collections.Generic;
using ActionsCollections;
using CustomeEditorTools;
using Game.Models;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static DebugConfigSection DebugConfig {get;} = new DebugConfigSection();}
    public class DebugConfigSection : SectionBase
    {
        public override string SectionName => "DebugConfig";
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new DebugConfigActionSetActiveDebugPrefab("Spawn benchmark on start: true","Benchmark",true));
            answer.Add(new DebugConfigActionSetActiveDebugPrefab("Spawn benchmark on start: false","Benchmark",false));

            return answer;
        }

    }
}