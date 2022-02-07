using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using Game.Providers;

namespace DebugActions
{
    public partial class DebugSections {public static QuestSection QuestSection {get;} = new QuestSection();}
    public class QuestSection : SectionBase
    {
        public override string SectionName => "QuestSection";
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private MainQuestsProvider MainQuestsProvider => ModelsSystem.Instance._mainQuestsProvider;

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new QuestActionSwitchSkeletonTestConfig("Switch Skeleton config"));
            answer.Add(new QuestActionSwitchShowTrigger("Switch Show Trigger"));
            SetQuestSection();

            answer.Add(new QuestActionSetStage("Stage 1",10));
            answer.Add(new QuestActionSetStage("Stage 2",20));
            answer.Add(new QuestActionSetStage("Stage 3",30));
            answer.Add(new QuestActionSetStage("Stage 4",40));
            answer.Add(new QuestActionSetStage("Stage 5",50));
            answer.Add(new QuestActionSetStage("Stage 6",60));
            answer.Add(new QuestActionSetStage("Stage 7",70));
            answer.Add(new QuestActionSetStage("Stage 8",80));
            answer.Add(new QuestActionSetStage("Stage 9",90));

            return answer;


            void SetQuestSection()
            {
                foreach (var questIndex in MainQuestsProvider.QuestsIndexes)
                {
                    answer.Add(new QuestActionSetQuest($"Quest{questIndex}",questIndex));
                }
            }
        }

    }
}