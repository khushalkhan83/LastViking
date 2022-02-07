using System.Collections.Generic;
using ActionsCollections;

namespace DebugActions
{
    public partial class DebugSections {public static EncountersSection Encounters {get;} = new EncountersSection();}
    public class EncountersSection : SectionBase
    {
        public override string SectionName => "Encounters";

        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new EncountersActionReset("Reset"));

            return answer;
        }

    }


}