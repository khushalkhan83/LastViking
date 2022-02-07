using System.Collections.Generic;

namespace ActionsCollections
{
    public abstract class SectionBase
    {
        public abstract string SectionName { get; }
        public abstract List<ActionBase> Actions { get; }
    }
}
