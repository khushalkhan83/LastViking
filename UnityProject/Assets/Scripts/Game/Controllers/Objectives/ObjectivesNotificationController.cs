using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System.Collections.Generic;
using System.Linq;

namespace Game.Controllers
{
    public class ObjectivesNotificationController : IController, IObjectivesNotificationController
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public ObjectivesNotificationModel ObjectivesNotificationModel { get; private set; }

        public Dictionary<byte, ObjectiveModel> Objectives { get; } = new Dictionary<byte, ObjectiveModel>();
        public Dictionary<ObjectiveModel, byte> ObjectiveIds { get; } = new Dictionary<ObjectiveModel, byte>();

        void IController.Enable()
        {
            ObjectivesNotificationModel.OnShow += OnShowHandler;
            ObjectivesModel.OnRemoveObjective += OnRemoveObjectiveHandler;
        }

        private void OnRemoveObjectiveHandler(ObjectiveModel obj)
        {
            if (!Objectives.ContainsValue(obj))
            {
                ClearObjectivesData();
                InitObjectivesData();
            }
        }

        void IController.Start()
        {
            InitObjectivesData();

            PlayerObjectivesModel.OnCreate += OnCreateHandler;
            PlayerObjectivesModel.OnPreEndTask += OnEndTaskHandler;
        }

        void IController.Disable()
        {
            PlayerObjectivesModel.OnCreate -= OnCreateHandler;
            PlayerObjectivesModel.OnPreEndTask -= OnEndTaskHandler;

            ObjectivesNotificationModel.OnShow -= OnShowHandler;
            ObjectivesModel.OnRemoveObjective -= OnRemoveObjectiveHandler;

            foreach (var item in ObjectiveIds)
            {
                item.Key.OnProgress -= OnProgressObjectiveHandler;
                item.Key.OnComplete -= OnProgressObjectiveHandler;
            }
            ObjectiveIds.Clear();
            Objectives.Clear();
        }

        private void ClearObjectivesData()
        {
            for (int i = Objectives.Count - 1; i >= 0; i--)
            {
                var obj = Objectives.ElementAt(0);
                Unregister(obj.Key, obj.Value);
            }

            ObjectivesNotificationModel.ClearDatas();
        }

        private void InitObjectivesData()
        {
            byte id = 0;

            foreach (var item in PlayerObjectivesModel.Pool)
            {
                if (item.IsHasValue)
                {
                    bool error = !ObjectivesModel.TryGet(item.Value, out var model);
                    if(error) continue;

                    Register(id, model);
                    ObjectivesNotificationModel.Create(item.Value);
                }

                ++id;
            }
        }

        private void OnShowHandler(ushort id) => ShowNotification(ObjectivesModel.Get(id));

        void OnCreateHandler(byte id)
        {
            var objectiveId = PlayerObjectivesModel.Pool[id].Value;
            var objective = ObjectivesModel.Get(objectiveId);

            Register(id, objective);
            ObjectivesNotificationModel.Create(objectiveId);
        }

        private void Register(byte id, ObjectiveModel objective)
        {
            Objectives.Add(id, objective);
            ObjectiveIds.Add(objective, id);
            objective.OnProgress += OnProgressObjectiveHandler;
            objective.OnComplete += OnProgressObjectiveHandler;
        }

        void OnEndTaskHandler(byte id)
        {
            bool error = !Objectives.TryGetValue(id, out var objective);
            if(error) return;

            Unregister(id, objective);
            ObjectivesNotificationModel.Close(PlayerObjectivesModel.Pool[id].Value);
        }

        private void Unregister(byte id, ObjectiveModel objective)
        {
            Objectives.Remove(id);
            ObjectiveIds.Remove(objective);
            objective.OnProgress -= OnProgressObjectiveHandler;
            objective.OnComplete -= OnProgressObjectiveHandler;
        }

        private void OnProgressObjectiveHandler(ObjectiveModel objectiveModel)
        {
            var objectiveId = PlayerObjectivesModel.Pool[ObjectiveIds[objectiveModel]].Value;
            ObjectivesNotificationModel.ChangeProgress(objectiveId, objectiveModel.Progress);
        }

        private void ShowNotification(ObjectiveModel objectiveModel)
        {
            var dataViewController = new NotificationObjectiveViewControllerData(objectiveModel);
            NotificationContainerViewModel.Show(PriorityID.TierObjectives, ViewConfigID.NotificationObjectiveView, dataViewController);
        }
    }
}
