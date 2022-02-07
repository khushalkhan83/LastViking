using System.Collections.Generic;
using ActionsCollections;
using CustomeEditorTools;
using Game.Models;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static PerfomanceSection Perfomance {get;} = new PerfomanceSection();}
    public class PerfomanceSection : SectionBase
    {
        public override string SectionName => "Perfomance";
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new PerfomanceActionDisplayAssetFileSizeToConsole<Texture>("Textures", 50));
            answer.Add(new PerfomanceActionDisplayAssetFileSizeToConsole<AudioClip>("AudioClip", 50));
            answer.Add(new PerfomanceActionDisplayAssetFileSizeToConsole<Mesh>("Mesh", 50));

            answer.Add(new PerfomanceActionDisplayAssetFileSizeToConsole<Texture>("Textures Full",true));
            answer.Add(new PerfomanceActionDisplayAssetFileSizeToConsole<AudioClip>("AudioClip Full",true));
            answer.Add(new PerfomanceActionDisplayAssetFileSizeToConsole<Mesh>("Mesh Full",true));

            return answer;
        }
    }
}