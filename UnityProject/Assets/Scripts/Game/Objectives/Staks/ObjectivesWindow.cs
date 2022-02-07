using System.Collections.Generic;
using Game.Objectives;
using NaughtyAttributes;
using UnityEngine;
using Extensions;
using System.Linq;
using System;
using Game.Models;
using static Game.Models.ObjectivesProgressModel;
using Game.Objectives.Stacks.Predefined;

namespace Game.Objectives.Stacks
{
    [CreateAssetMenu(fileName = "ObjectivesWindow", menuName = "UnityProject/ObjectivesWindow", order = 0)]
    public class ObjectivesWindow : ScriptableObject
    {
        #region Data
#pragma warning disable 0649
        [Header("Settings")]
        [SerializeField] private PredefinedStacks requiredObjectives = new PredefinedStacks();
        [SerializeField] private List<ObjectivesStack> highPriorityStacks;
        [SerializeField] private List<ObjectivesStack> staks;
        [SerializeField] private int highPriorityObjectives = 1; //TODO: handle for 
        [SerializeField] private int objectivesToDisplay = 4; //TODO: handle for // FIXME: refactor. move to prop. check logic in other places

        [Header("Runtime")]
        [SerializeField] private List<ObjectivesStack.OjbectiveStackData> selectedObjectives;
        [SerializeField] private ObjectivesStack.OjbectiveStackData targetObjective;

        [Header("Debug")]
        [SerializeField] private bool autoCompleateOnNextStage;

#pragma warning restore 0649
        #endregion

        #region Private properties
        // System related
        private ObjectivesProgressModel ObjectivesProgressModel => ModelsSystem.Instance._objectivesProgressModel;
        private ObjectivesProvider ObjectivesProvider => ModelsSystem.Instance._objectivesProvider;
        private StacksProvider StacksProvider => ModelsSystem.Instance._stacksProvider;
        private PlayerObjectivesModel PlayerObjectivesModel => ModelsSystem.Instance._playerObjectivesModel;
        private ObjectivesWindowModel ObjectivesWindowModel => ModelsSystem.Instance._objectivesWindowModel;

        // Settings
        private PredefinedStacks RequiredObjectives => requiredObjectives;
        private List<ObjectivesStack> HighPriorityStacks { get => highPriorityStacks; }
        private List<ObjectivesStack> Staks { get => staks; }
        private int ObjectivesToDisplay { get => objectivesToDisplay; }
        private int HighPriorityObjectives { get => highPriorityObjectives; }


        // Runtime
        private List<ObjectivesStack.OjbectiveStackData> SelectedObjectivesData
        {
            get
            {
                List<ObjectivesStack.OjbectiveStackData> answer = new List<ObjectivesStack.OjbectiveStackData>();
                foreach (var objectiveData in selectedObjectives)
                {
                    if(objectiveData == null || objectiveData.Objective == null)
                    {
                        Debug.Log("log");
                        continue;
                    }
                    answer.Add(objectiveData);
                }
                return answer;
            }
            set => selectedObjectives = value;
        }
        private ObjectivesStack.OjbectiveStackData TargetObjective { get => targetObjective; set => targetObjective = value; }

        // Debug
        private bool AutoCompleateOnNextStage => 
        #if UNITY_EDITOR
            autoCompleateOnNextStage;
        #else
            false;
        #endif

        #endregion

        #region Public properties

        public List<ObjectivesStack.OjbectiveStackData> SelectedObjectives => SelectedObjectivesData;
        public int RequiredTiersCount => RequiredObjectives.TiersCount();
        public List<bool> RequiredTiersStates => RequiredObjectives.RequiredTiersStates;
        public bool RequiredObjectivesCompleated => RequiredObjectives.Compleated;

        #endregion

        #region Debug Buttons

        [Button] void Test_NextTier()
        {
            PlayerObjectivesModel.NextTier();
        }

        [Button] void TestNull()
        {
            selectedObjectives[1] = null;
        }

        #endregion

        #region Public Interface

        public void RerollObjective(ObjectiveData objectiveData, int index)
        {
            var objectivesExeptCompleated = SelectedObjectivesData.Where(x => !x.Done).ToList();
            ObjectivesStack.OjbectiveStackData match;

            match = objectivesExeptCompleated.Find(x => x.Objective == objectiveData);


            if (match == null)
            {
                Debug.LogError("Cant find match here");
                return;
            }

            if (match.UseSameStackOnReroll())
            {
                var targetData = SelectedObjectivesData.Find(x => x.Objective == objectiveData);
                var newData = targetData.GetOjbectiveStackDataFromSameStack();

                if(newData == null) return;

                var indexOfMatch = SelectedObjectivesData.IndexOf(match);
                var temp = SelectedObjectivesData;

                temp[indexOfMatch] = newData;
                SelectedObjectivesData = temp;
            }
            else
            {
                var randomObjectives = GetNextObjectivesFromDiffrentStacks(doNotUseSelectedObjectives: true);
                var randomObjective = randomObjectives.RandomElement();

                if(randomObjective == null) return;

                var indexOfMatch = SelectedObjectivesData.IndexOf(match);
                var temp = SelectedObjectivesData;
                temp[indexOfMatch] = randomObjective;
                SelectedObjectivesData = temp;

            }
        }

        public void SelectRandomObjectives()
        {
            SelectedObjectivesData = GetNextObjectivesFromDiffrentStacks();
            SetNextTargetObjective();
        }

        private void SetNextTurn()
        {
            if (!RequiredObjectives.Compleated)
            {
                if (RequiredObjectives.FirstInit)
                {
                    RequiredObjectives.FirstInit = false;
                }
                else
                {
                    RequiredObjectives.CompleateSelectedTierAndSetNext();
                }
            }

            if (!RequiredObjectives.Compleated)
            {
                var predefinedObjectives = GetObjectivesFromPredefinedStacks();
                SelectedObjectivesData = predefinedObjectives;
                SetNextTargetObjective();
            }
            else
            {
                SelectRandomObjectives();
            }

            RefreshSelection();
        }
        public void NextTurn()
        {

            for (int i = 0; i < ObjectivesToDisplay; i++)
            {
                if (AutoCompleateOnNextStage)
                    TargetObjective?.Complete();

                TargetObjective?.TryRefresh();
                SetNextTargetObjective();
            }

            SetNextTurn();
        }


        public void Reset() // called not only by objectivesWindowModel
        {
            // load selected objectives to refresh them 
            LoadFromSaveSelectedObjectives();
            // updates IsSelected for each stack. 
            // ObjectiveStack.GetIsDonePerObjective need this property for conditionalObjectives
            RefreshSelection();

            RequiredObjectives.Reset();
            Staks.ForEach(x => x.RegenerateStack());
            HighPriorityStacks.ForEach(x => x.RegenerateStack());

            // load selected objectives, because data was changed
            LoadFromSaveSelectedObjectives();

            ResetObjectiveDataConditions();
        }

        public void UpdateConditions()
        {
            Staks.ForEach(x => x.UpdateConditions());
            HighPriorityStacks.ForEach(x => x.UpdateConditions());
        }

        public void RerollStack(ObjectivesStack stack)
        {
            var stackToReroll = SelectedObjectivesData.Find(x => stack.Datas.Contains(x));

            if (stackToReroll == null)
            {
                Debug.LogError("cant reroll");
                return;
            }

            var objectives = GetNextObjectivesFromDiffrentStacks();
            var newObjective = objectives.RandomElement();

            SelectedObjectivesData[SelectedObjectivesData.FindIndex(x => x.Objective == stackToReroll.Objective)] = newObjective;

            RefreshSelection();
        }

        public void CompleteObjectiveInStack(ObjectivesStack stack)
        {
            var target = SelectedObjectivesData.Find(x => stack.Datas.Contains(x));

            if (target == null)
            {
                Debug.LogError("target null");
                return;
            }

            target.Complete();
        }

        // FIXME: fix dublicates in selected objectives, bacause reroll objectives can work wrong
        public bool CanRerollObjective(ObjectiveData objectiveData)
        {
            if (RequiredObjectives.Compleated) return true;

            var match = SelectedObjectivesData.Find(x => x.Objective == objectiveData);
            if (match == null) return true;
            var objectiveIndex = SelectedObjectivesData.IndexOf(match);

            var noErrors = RequiredObjectives.TrtGetCanRerollObjective(objectiveIndex, out bool canReroll);

            if (noErrors) return canReroll;
            else return true;
        }

        #endregion

        #region Private methods

        private void ResetObjectiveDataConditions()
        {
            var ids = Helpers.EnumsHelper.GetValues<ObjectiveID>().ToList();
            ids.Remove(ObjectiveID.None);

            foreach (var id in ids)
            {
                var objective = ObjectivesProvider[id];

                var conditionID = objective.Conditions.FirstOrDefault().Id;
                var state = ObjectivesProgressModel.GetObjestiveState(conditionID);

                objective.Done = state;
            }
        }
        private void LoadFromSaveSelectedObjectives()
        {
            SelectedObjectivesData.Clear();
            SelectedObjectivesData = GetObjectivesFromSaveFile();
        }

        private void RefreshSelection()
        {
            SetAllStacksAsNotSelected();
            SetSelectedObjectivesStaksAsSelected();
        }

        private void SetAllStacksAsNotSelected()
        {
            Staks.ForEach(x => x.IsSelected = false);
            HighPriorityStacks.ForEach(x => x.IsSelected = false);
        }

        private void SetSelectedObjectivesStaksAsSelected()
        {
            foreach (var selectedObjective in SelectedObjectivesData)
            {
                selectedObjective.MarkStackAsSelected();
            }
        }

        private void SetNextTargetObjective()
        {
            List<ObjectivesStack.OjbectiveStackData> validObjectives = new List<ObjectivesStack.OjbectiveStackData>();
            foreach (var selectedObjective in SelectedObjectivesData)
            {
                if (selectedObjective == null)
                {
                    Debug.LogError("Here");
                }
                if (selectedObjective.ElementIsValid())
                    validObjectives.Add(selectedObjective);
            }
            if (validObjectives.Count != 0)
                TargetObjective = validObjectives.RandomElement();

            RefreshSelection();
        }

        private List<ObjectivesStack.OjbectiveStackData> GetObjectivesFromPredefinedStacks()
        {
            var answer = new List<ObjectivesStack.OjbectiveStackData>();
            var currentTier = RequiredObjectives.SelectedTier;

            currentTier.Stacks.ForEach(x => answer.Add(x.GetElement()));
            return answer;
        }

        private List<ObjectivesStack.OjbectiveStackData> GetNextObjectivesFromDiffrentStacks(bool doNotUseSelectedObjectives = false)
        {
            var answer = new List<ObjectivesStack.OjbectiveStackData>();
            var regularStacks = new List<ObjectivesStack>();
            var highPriorityStacks = new List<ObjectivesStack>();
            regularStacks.AddRange(Staks);
            highPriorityStacks.AddRange(this.HighPriorityStacks);

            #region Logic
            for (int i = 0; i < ObjectivesToDisplay; i++)
            {

                if (i < HighPriorityObjectives)
                {
                    bool error = !HandleHithPriorityStack();
                    if (error)
                    {
                        HandleRegularStack();
                    }
                }
                else
                {
                    HandleRegularStack();
                }
            }

            int freeSlots = ObjectivesToDisplay - answer.Count;
            HandleEmptySlots();

            return answer;
            #endregion

            #region  Handlers

            bool HandleHithPriorityStack() => HandleStacks(ref highPriorityStacks);
            bool HandleRegularStack() => HandleStacks(ref regularStacks);

            //FIXME: add valiation to exlude high priority and regular overlap
            //FIXME: add validation to exlude overlap in one category
            //FIXME: add logic to handle potential dublicates in next objectives.

            bool HandleStacks(ref List<ObjectivesStack> targetStacks)
            {
                var stacksWithNotCompleatedObjectives = targetStacks.
                                        Where(x => x.Datas.Where(y => !y.Done).Count() != 0).ToList();

                var stacksWithNotCompleatedObjectivesAndOkConditions = stacksWithNotCompleatedObjectives.
                                        Where(x => x.Datas.Where(y => y.ConditionsOk).Count() != 0).ToList();


                if (doNotUseSelectedObjectives)
                {
                    RemoveAlreadySelectedStacks(stacksWithNotCompleatedObjectivesAndOkConditions, out var filteredStacks);
                    stacksWithNotCompleatedObjectivesAndOkConditions = filteredStacks;
                }


                if (stacksWithNotCompleatedObjectivesAndOkConditions.Count == 0) return false;
                var randomStack = stacksWithNotCompleatedObjectivesAndOkConditions.RandomElement();

                targetStacks.Remove(randomStack); // FIXME: here to change code
                var nextObjective = randomStack.GetElement();
                answer.Add(nextObjective);
                return true;
            }

            void RemoveAlreadySelectedStacks(List<ObjectivesStack> stacks, out List<ObjectivesStack> filteredStgacks)
            {
                filteredStgacks = new List<ObjectivesStack>();
                var selectedObjectives = SelectedObjectivesData.Select(x => x.Objective).ToList();

                bool noMatch;
                foreach (var stack in stacks)
                {
                    noMatch = true;
                    foreach (var data in stack.Datas)
                    {
                        if (selectedObjectives.Contains(data.Objective))
                        {
                            noMatch = false;
                            break;
                        }
                    }
                    if (noMatch) filteredStgacks.Add(stack);
                }
            }

            void HandleEmptySlots()
            {
                if (freeSlots < 0) Debug.LogError("Error here");

                RemoveAlreadySelectedStacks(regularStacks, out var filteredStacks);

                for (int i = 0; i < freeSlots; i++)
                {
                    var randomStack = filteredStacks.RandomElement();
                    var objectiveFromStack = randomStack.GetElement_LateGame();
                    objectiveFromStack.ForceRefresh();
                    answer.Add(objectiveFromStack);
                }
            }
            #endregion
        }

        private List<ObjectivesStack.OjbectiveStackData> GetObjectivesFromSaveFile()
        {
            List<ObjectivesStack.OjbectiveStackData> answer = new List<ObjectivesStack.OjbectiveStackData>();
            List<StackProgress> datas = ObjectivesProgressModel.GetStackDatas();

            for (int i = 0; i < datas.Count; i++)
            {
                var id = datas[i].id;
                var stackById = StacksProvider[id]; // FIXME: use TryGetValue //TODO: add error handling (if else)

                ObjectivesStack.OjbectiveStackData dataForSelectedStack = stackById.GetSavedElement(datas[i]); // TODO: add objective data is null check
                
                if(dataForSelectedStack != null)
                    answer.Add(dataForSelectedStack);
            }

            return answer;
        }

        #endregion
    }
}