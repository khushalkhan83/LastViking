using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System.Collections.Generic;

namespace Game.Controllers
{
    public class ObjectivesButtonViewController : ViewControllerBase<ObjectivesButtonView>
    {
        [Inject] public EnableObjectivesButtonModel EnableObjectivesButtonModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        private Dictionary<byte, ObjectiveModel> Objectives { get; } = new Dictionary<byte, ObjectiveModel>();

        protected override void Show()
        {
            View.OnClick += OnClickHandler;
            View.SetAttentionIconVisible(false);
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.Objectives, OnHideObjectivesViewHandler);

            RegisterObjectives();
            UpdateViewState();

            ObjectivesWindowModel.OnCompleatedObjective += OnRecivedRewardHandler;
        }

        protected override void Hide()
        {
            View.OnClick -= OnClickHandler;
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.Objectives, OnHideObjectivesViewHandler);

            ObjectivesWindowModel.OnCompleatedObjective -= OnRecivedRewardHandler;

            UnRegisterObjectives();
        }

        #region Handlers
            
        private void OnRecivedRewardHandler(ObjectiveModel removedModel)
        {
            if(removedModel == null) return;

            UnRegisterObjectives();
            RegisterObjectives();
           
            UpdateViewState();
        }

        private void OnCompleteObjectiveHandler(ObjectiveModel objectiveModel)
        {
            UpdateViewState();
            objectiveModel.OnComplete -= OnCompleteObjectiveHandler;
        }

        private void OnClickHandler()
        {
            View.SetIsInteractable(false);
            var objectivesView = ViewsSystem.Show<ObjectivesView>(ViewConfigID.Objectives);
        }

        private void OnHideObjectivesViewHandler()
        {
            EnableObjectivesButtonModel.ResetFirst();
            View.SetIsInteractable(true);
            UpdateViewState();
        }
        #endregion

        private void UpdateViewState()
        {
            if (EnableObjectivesButtonModel.IsFirstShow || HasRewards())
            {
                View.PlayPulse();
            }
            else
            {
                View.PlayDefault();
            }
        }

        private bool HasRewards()
        {
            foreach (var objective in Objectives)
            {
                if(objective.Value == null) continue;
                if(objective.Value.ObjectiveData == null) continue;
                if(objective.Value.ObjectiveData.Done == false && objective.Value.IsComplete) return true;
            }
            return false;
        }

        private void RegisterObjectives()
        {
            for (byte id = 0; id < PlayerObjectivesModel.Pool.Length; id++)
            {
                RegisterObjective(id);
            }
        }

        private void RegisterObjective(byte id)
        {
            if (PlayerObjectivesModel.Pool[id].IsHasValue)
            {
                bool result = TryGetObjective(id, out var objectiveModel);
                if (result)
                {
                    Objectives[id] = objectiveModel;
                    objectiveModel.OnComplete += OnCompleteObjectiveHandler;
                }
            }
        }

        private bool TryGetObjective(byte id, out ObjectiveModel objectiveModel)
        {
            bool ok = ObjectivesModel.TryGet(PlayerObjectivesModel.Pool[id].Value, out objectiveModel);
            return ok;
        }

        private void UnRegisterObjective(byte id)
        {
            if (Objectives.ContainsKey(id))
            {
                Objectives[id].OnComplete -= OnCompleteObjectiveHandler;
                Objectives.Remove(id);
            }
        }

        private void UnRegisterObjectives()
        {
            foreach (var objective in Objectives.Values)
            {
                if(objective == null) continue;

                objective.OnComplete -= OnCompleteObjectiveHandler;
            }

            Objectives.Clear();
        }
    }
}
