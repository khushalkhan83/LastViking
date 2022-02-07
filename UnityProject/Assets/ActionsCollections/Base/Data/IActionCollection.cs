using System.Collections.Generic;

namespace ActionsCollections
{
    public interface IActionCollection
    {
        List<SectionBase> Sections { get; }
    }
}