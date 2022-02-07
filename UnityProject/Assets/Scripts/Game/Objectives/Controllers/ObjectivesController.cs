using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives.Data;
using Game.Objectives.Data.Conditions.Static;
using System;
using System.Collections.Generic;

namespace Game.Objectives.Controllers
{
    public class ObjectivesController : IController, IObjectivesController
    {
        [Inject] public ConditionDataMapper ConditionDataMapper { get; private set; }
        [Inject] public ObjectivesProvider ObjectivesProvider { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel { get; private set; }

        void IController.Enable()
        {
            ObjectivesModel.OnUnpackData += UnpackData;
            ObjectivesModel.OnPackData += PackData;
            ObjectivesModel.OnCreate += Create;

            StorageModel.TryProcessing(ObjectivesModel.ObjectivesModelData);
            ObjectivesModel.UnpackData();

            StorageModel.OnPackData += OnPackDataHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            ObjectivesModel.OnUnpackData -= UnpackData;
            ObjectivesModel.OnPackData -= PackData;
            ObjectivesModel.OnCreate -= Create;

            StorageModel.OnPackData -= OnPackDataHandler;
        }

        private void OnPackDataHandler() => ObjectivesModel.PackData();

        private ConditionModel[] CreateConditionModels(ConditionBaseData[] conditions)
        {
            var result = new ConditionModel[conditions.Length];

            ConditionBaseData conditionBaseData;

            for (int i = 0; i < result.Length; i++)
            {
                conditionBaseData = conditions[i];
                result[i] = new ConditionModel
                (
                    conditionBaseData.ConditionID,
                    CreateConditionDataModel(ConditionDataMapper[conditionBaseData.ConditionID]),
                    conditionBaseData
                );
            }

            return result;
        }

        public ObjectiveModel Create(ObjectiveID objectiveID)
        {
            bool canReroll = ObjectivesWindowModel.CanRerollObjective(objectiveID);

            var objective = ObjectivesProvider[objectiveID];

            return new ObjectiveModel(objectiveID, objective, CreateConditionModels(objective.Conditions),canReroll);
        }

        public ObjectivesData PackData(IEnumerable<ObjectiveModel> objectiveModels)
        {
            var conditionTypesCount = GetConditionTypesCount(objectiveModels);
            var objectiveDatas = new Data.ObjectiveData[conditionTypesCount.Count];
            var intDatas = new int[conditionTypesCount.CountInt];
            var boolDatas = new bool[conditionTypesCount.CountBool];

            ushort objectiveDataId = 0;
            ushort intDataId = 0;
            ushort boolDataId = 0;
            int conditionCount;
            ConditionID conditionID;
            ConditionDataTypeID conditionDataTypeID;
            ConditionModel[] conditionModels;
            ConditionModel conditionModel;
            ConditionData[] conditionDatas;

            foreach (var objectiveModel in objectiveModels)
            {
                conditionModels = objectiveModel.Conditions;
                conditionCount = conditionModels.Length;
                conditionDatas = new ConditionData[conditionCount];

                for (byte i = 0; i < conditionCount; i++)
                {
                    conditionModel = conditionModels[i];
                    conditionID = conditionModel.ConditionID;
                    conditionDataTypeID = ConditionDataMapper[conditionID];

                    conditionDatas[i].ConditionID = conditionID;

                    if (conditionDataTypeID == ConditionDataTypeID.Int)
                    {
                        conditionDatas[i].Id = intDataId;
                        intDatas[intDataId] = ((IProgress<int>)conditionModel.ConditionDataModel).Value;
                        ++intDataId;
                    }
                    else if (conditionDataTypeID == ConditionDataTypeID.Bool)
                    {
                        conditionDatas[i].Id = boolDataId;
                        boolDatas[boolDataId] = ((IProgress<bool>)conditionModel.ConditionDataModel).Value;
                        ++boolDataId;
                    }
                }

                objectiveDatas[objectiveDataId].Id = objectiveDataId;
                objectiveDatas[objectiveDataId].Conditions = conditionDatas;
                objectiveDatas[objectiveDataId].ObjectiveID = objectiveModel.ObjectiveID;

                ++objectiveDataId;
            }

            return new ObjectivesData()
            {
                ObjectivesDatas = objectiveDatas,
                IntDatas = intDatas,
                BoolDatas = boolDatas
            };
        }

        public (int Count, int CountInt, int CountBool) GetConditionTypesCount(IEnumerable<ObjectiveModel> objectiveModels)
        {
            var countInt = 0;
            var countBool = 0;
            var count = 0;

            foreach (var objective in objectiveModels)
            {
                foreach (var condition in objective.Conditions)
                {
                    switch (condition.ConditionDataModel.ConditionDataTypeID)
                    {
                        case ConditionDataTypeID.Int:
                            ++countInt;
                            break;
                        case ConditionDataTypeID.Bool:
                            ++countBool;
                            break;
                    }
                }
                ++count;
            }

            return (count, countInt, countBool);
        }

        public ObjectiveModel[] UnpackData(ObjectivesData data)
        {
            var result = new ObjectiveModel[data.ObjectivesDatas.Length];
            var objectiveId = 0;

            int countCondition;
            ConditionData[] conditionDatas;
            ObjectiveData objectiveData;
            ConditionBaseData conditionBaseData;
            ConditionBaseData[] conditionBaseDatas;
            ConditionModel[] conditionModels;

            foreach (var objective in data.ObjectivesDatas)
            {
                conditionDatas = objective.Conditions;
                objectiveData = ObjectivesProvider[objective.ObjectiveID];
                conditionBaseDatas = objectiveData.Conditions;
                countCondition = conditionBaseDatas.Length;

                conditionModels = new ConditionModel[countCondition];

                for (int i = 0; i < countCondition; i++)
                {
                    conditionBaseData = conditionBaseDatas[i];

                    conditionModels[i] = new ConditionModel
                    (
                        conditionDatas[i].ConditionID,
                        CreateConditionDataModel
                        (
                            data,
                            conditionDatas[i].Id,
                            ConditionDataMapper[conditionDatas[i].ConditionID]
                        ),
                        conditionBaseData
                    );
                }

                bool canReroll = true;

                result[objectiveId] = new ObjectiveModel(objective.ObjectiveID, objectiveData, conditionModels,canReroll);
                ++objectiveId;
            }

            return result;
        }

        private static IConditionDataModel CreateConditionDataModel(ConditionDataTypeID conditionTypeID)
        {
            switch (conditionTypeID)
            {
                case ConditionDataTypeID.Int:
                    return new CountConditionDataModel();
                case ConditionDataTypeID.Bool:
                    return new DoneConditionDataModel();
            }

            throw new Exception();
        }

        private static IConditionDataModel CreateConditionDataModel(ObjectivesData data, ushort id, ConditionDataTypeID conditionDataTypeID)
        {
            switch (conditionDataTypeID)
            {
                case ConditionDataTypeID.Int:
                    return new CountConditionDataModel(data.IntDatas[id]);
                case ConditionDataTypeID.Bool:
                    return new DoneConditionDataModel(data.BoolDatas[id]);
            }

            throw new Exception();
        }
    }
}
