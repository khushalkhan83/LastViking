using System.Collections.Generic;
using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static FrameRateSection FrameRate {get;} = new FrameRateSection();}
    public class FrameRateSection : SectionBase
    {
        public override string SectionName => "FrameRate";
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new FrameRateActionVsync("vSyncCount 0",0));
            answer.Add(new FrameRateActionVsync("vSyncCount 1",1));
            answer.Add(new FrameRateActionVsync("vSyncCount 2",2));
            answer.Add(new FrameRateActionVsync("vSyncCount 3",3));
            answer.Add(new FrameRateActionVsync("vSyncCount 4",4));

            answer.AddRange(GetFrameRateRange(10,200,2));

            return answer;
        }

        private List<ActionBase> GetFrameRateRange(int start, int end, int step)
        {
            var answer = new List<ActionBase>();
            for (int i = start; i < end; i += step)
            {
                var newButton = new FrameRateActionGeneric(i.ToString(),i);
                answer.Add(newButton);
            }
            return answer;
        }
    }


}