using Game.Objectives;
using UnityEngine;
using NaughtyAttributes;
using Game.Models;
using static Game.Models.ObjectivesProgressModel;

namespace Game.Objectives.Stacks
{
    public partial class ObjectivesStack
    {
        [System.Serializable]
        public class OjbectiveStackData
        {
            // serialized properties in this case used to visualy see private variables in inspector
            #region Data
            [SerializeField] private string objectiveName;
            [SerializeField] private ObjectiveData objective;
            [SerializeField] private bool conditionsOk; // TODO: make it property
            [SerializeField] private bool done; // TODO: make it property

            [ReadOnly] [SerializeField] private ObjectivesStack stack;
            [ReadOnly] [SerializeField] private ObjectivesStack compoziteStack;

            #endregion

            #region Private properties
            private StacksProvider StacksProvider => ModelsSystem.Instance._stacksProvider;

            #endregion

            #region Public properties
            public string ObjectiveName => Objective.name;
            public ObjectiveData Objective { get => objective; }
            public bool ConditionsOk { get => conditionsOk; }
            public bool Done { get => done; }

            #endregion

            #region Public Methods: change/Set data

            public OjbectiveStackData(ObjectiveData objective, bool done, ObjectivesStack stack, bool conditionsOk)
            {
                this.objective = objective;
                this.done = done;
                this.stack = stack;
                this.conditionsOk = conditionsOk;
                UpdateName();
            }

            public void SetConditionsOk(bool value) => conditionsOk = value;

            public void SetCompoziteStack(ObjectivesStack compoziteStack) => this.compoziteStack = compoziteStack;

            public void Complete()
            {
                done = true;
                Objective.Done = true;
                UpdateName();
                stack.Update();
                compoziteStack?.Update();
            }

            public void TryRefresh()
            {
                if (!stack.ElementIsRefreshable()) return;

                ForceRefresh();
            }

            public void ForceRefresh()
            {
                done = false;
                Objective.Done = false;
                UpdateName();
                stack.Update();
                compoziteStack?.Update();
            }

            public void MarkStackAsSelected() => stack.IsSelected = true;

            #endregion

            #region Public Methods: Get data

            public StackProgress GetStackData()
            {
                StackProgress stackData;
                var stackType = stack.StackType;
                var stackID = GetStackID(stack);
                var objectiveName = ObjectiveName;

                switch (stackType)
                {
                    case ObjectivesStackType.Linear:
                        stackData = new StackProgress(stackID, objectiveName);
                        return stackData;
                    case ObjectivesStackType.Random:
                        stackData = new StackProgress(stackID, objectiveName);
                        return stackData;
                    case ObjectivesStackType.Conditional:
                        stackData = new StackProgress(stackID, objectiveName);
                        return stackData;
                    case ObjectivesStackType.Compozite:
                        int compoziteStackID = GetStackID(compoziteStack);
                        stackData = new StackProgress(stackID, objectiveName, compoziteStackID);
                        return stackData;
                    default:
                        Debug.LogError("Error here");
                        return null;
                }
            }

            public bool ElementIsValid()
            {
                bool result = stack.ElementIsValid(this);
                return result;
            }

            public bool UseSameStackOnReroll()
            {
                var targetStack = compoziteStack != null ? compoziteStack : stack;
                bool result = targetStack.UseSameStackOnReroll();
                return result;
            }

            public OjbectiveStackData GetOjbectiveStackDataFromSameStack()
            {
                var answer = stack.GetElement();
                return answer;
            }

            #endregion

            #region  Private methods

            private void UpdateName() => objectiveName = Objective.name + (Done ? "(+)" : "(-)");

            private int GetStackID(ObjectivesStack stack)
            {
                var answer = StacksProvider.GetID(stack); // TODO: add stack type ?
                return answer;
            }

            #endregion
        }
    }
}