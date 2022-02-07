using Game.Objectives.Data.Conditions.Static;
using System;
using System.Collections.Generic;

namespace Game.Objectives.Conditions.Controllers
{
    abstract public class BaseConditionController<D, M> : IConditionController
        where D : ConditionBaseData
        where M : IConditionDataModel
    {
        public HashSet<ConditionModel> ConditionModels { get; } = new HashSet<ConditionModel>();
        public List<ConditionModel> Completed { get; } = new List<ConditionModel>();

        public void Register(ConditionModel conditionModel)
        {
            if (ConditionModels.Count == 0)
            {
                Subscribe();
            }

            ConditionModels.Add(conditionModel);
        }

        public void Unregister(ConditionModel conditionModel)
        {
            if (ConditionModels.Remove(conditionModel))
            {
                if (ConditionModels.Count == 0)
                {
                    Unsubscribe();
                }
            }
        }

        abstract protected void Subscribe();
        abstract protected void Unsubscribe();

        /// <summary>
        /// EventProcessing
        /// </summary>
        /// <param name="condition">condition for select models</param>
        /// <param name="progress">how to set progress in model</param>
        /// <param name="isComplete">condition for complete objective</param>
        protected void EventProcessing(Func<D, bool> condition, Action<M> progress, Func<D, M, bool> isComplete)
        {
            D data;
            M model;

            foreach (var conditionModel in ConditionModels)
            {
                data = (D)conditionModel.ConditionBaseData;
                model = (M)conditionModel.ConditionDataModel;

                if (condition(data))
                {
                    progress(model);

                    if (isComplete(data, model))
                    {
                        Completed.Add(conditionModel);
                    }
                }
            }

            foreach (var conditionModel in Completed)
            {
                Unregister(conditionModel);
                conditionModel.Complete();
            }
            Completed.Clear();
        }
    }
}
