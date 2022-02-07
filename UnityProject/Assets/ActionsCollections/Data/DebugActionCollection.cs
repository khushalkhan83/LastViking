using System.Collections.Generic;
using ActionsCollections;

namespace DebugActions
{
    public class DebugActionCollection : IActionCollection
    {
        private static DebugActionCollection instance;
        public static DebugActionCollection Instance
        {
            get
            {
                if(instance == null) instance = new DebugActionCollection();
                return instance;
            }
        }

        public List<SectionBase> Sections {get; private set;} = GetDefaultSections();
        public static List<SectionBase> GetDefaultSections() => new List<SectionBase>()
        {
            DebugSections.Dock,
            // DebugSections.DragTime,
            DebugSections.DebugConfig,
            DebugSections.QuestSection,
            DebugSections.Teleport,
            DebugSections.Scenes,
            DebugSections.Items,
            DebugSections.AI,
            DebugSections.DebugOptions,
            DebugSections.Encounters,
            
            DebugSections.Perfomance,
            DebugSections.Waterfall,
            DebugSections.Toggle,
            DebugSections.View,
            DebugSections.FrameRate,
            DebugSections.Addressables,
        };

        public void Reset() 
        {
            Sections = GetDefaultSections();
        }

        public void InsertSection(SectionBase section)
        {
            Sections.Insert(0,section);
        }
    }
    
}