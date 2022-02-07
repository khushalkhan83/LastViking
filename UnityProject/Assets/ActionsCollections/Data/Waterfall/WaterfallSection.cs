using ActionsCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections { public static WaterfallSection Waterfall {get;} = new WaterfallSection(); }
    public class WaterfallSection : SectionBase
    {
        public override string SectionName => "Waterfall";
        public override List<ActionBase> Actions
        {
            get => new List<ActionBase>()
                {
                    new WaterfallActionGeneric("Reset Waterfall"),
                };
        }
    }
}
