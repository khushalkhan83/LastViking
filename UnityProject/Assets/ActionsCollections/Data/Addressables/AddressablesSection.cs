using System.Collections.Generic;
using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static AddressablesSection Addressables {get;} = new AddressablesSection();}
    public class AddressablesSection : SectionBase
    {
        public override string SectionName => "Addressables";
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {

            var answer = GetFrameRateRange(0,10,1);

            return answer;
        }

        private List<ActionBase> GetFrameRateRange(int start, int end, int step)
        {
            var answer = new List<ActionBase>();
            for (int i = start; i < end; i += step)
            {
                var newButton = new AddressablesActionSetWeaponPoolSize("pool size" + i,i);
                answer.Add(newButton);
            }
            return answer;
        }
    }


}