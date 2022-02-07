using Game.AI.Behaviours.Kraken;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra.Kraken
{
    [DefaultExecutionOrder(-1)]
    public class KrakenActivation : MonoBehaviour
    {
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;
        private CutsceneKrakenModel CutsceneKrakenModel => ModelsSystem.Instance._cutsceneKrakenModel;
        private ApplicationCallbacksModel ApplicationCallbacksModel => ModelsSystem.Instance._applicationCallbacksModel;

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        #region Data
#pragma warning disable 0649
        [SerializeField] private bool doNotWaitForCutsceneActivation;
        [SerializeField] private KrakenConfig config;
#pragma warning restore 0649
        #endregion

        #region MonoBehaviour
        private void OnEnable()
        {
            if (config != null)
                FirstKrakenModel.SetConfig(config);

            if (doNotWaitForCutsceneActivation && (!FirstKrakenModel.Active || EditorGameSettings.ResetKrakenOnStart))
            {
                FirstKrakenModel.SetKrakenActive();
            }
        }

        private void OnDisable()
        {
            // call only when quest stage is changed not when application is quiting
            if (ApplicationCallbacksModel.IsApplicationQuitting) return;

            FirstKrakenModel.ResetData();
            CutsceneKrakenModel.SetShown(false);
        }
        #endregion
    }
}