using System.Collections.Generic;
using System.Linq;
using ActionsCollections;
using Game.Models;

namespace DebugActions
{
    public partial class DebugSections {public static ScenesSection Scenes {get;} = new ScenesSection();}
    public class ScenesSection : SectionBase
    {
        public override string SectionName => "Scenes";
        public override List<ActionBase> Actions
        {
            get => GetActions();
        }

        private List<ActionBase> GetActions()
        {
            var answer = new List<ActionBase>();

            answer.Add(new SceneActionGameFullRestart("FullRestart"));
            answer.Add(new SceneActionRestartCore("RestartCore"));
            answer.Add(new SceneActionRestartCore("RestartGame",true));

            var ids = Helpers.EnumsHelper.GetValues<EnvironmentSceneID>().ToList();
            ids.Remove(EnvironmentSceneID.None);
            foreach (var id in ids)
            {
                answer.Add(new SceneActionTransitionToScene("Go to: " + id.ToString(), id));
            }

            // answer.Add(new SceneActionSpawnSomething("SpawnKraken",true,"Spawners","KrakenSpawners"));
            answer.Add(new ScenesActionToggleWaitForChunkLoadedOption("WaitForChunk(On/Off"));
            answer.Add(new SceneActionToggleSetPauseOnScenesTransition("SetPauseOnTransitionStart(On/Off"));
            

            for (int i = 0; i < 15; i++)
            {
                answer.Add(new ScenesActionChangeLoadingTime("loadTime:" + i, i));
            }


            return answer;
        }

    }
}