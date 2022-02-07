using System.Collections.Generic;
using ActionsCollections;
using CustomeEditorTools;
using Game.Models;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static DragTimeSection DragTime {get;} = new DragTimeSection();}
    public class DragTimeSection : SectionBase
    {
        public override string SectionName => "Selec Cell Time";
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private const int SpawnDistance = 3;
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            float timeStep = 0.02f;
            float time = 0;
            
            for (int i = 0; i < 16; i++)
            {
                answer.Add(new DragTimeActionSetTime(time.ToString(),time));
                time += timeStep;
            }           


            return answer;
        }

    }


}