using System.Collections.Generic;
using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static ViewSection View => new ViewSection();}
    public class ViewSection : SectionBase
    {
        public override string SectionName => "View";
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {

            var answer = new List<ActionBase>();

            answer.Add(new ViewActionMoveToPopup("debug buttons on top"));
            answer.Add(new ViewActionResetObjectives("Reset objectives"));

            return answer;
        }
    }


}