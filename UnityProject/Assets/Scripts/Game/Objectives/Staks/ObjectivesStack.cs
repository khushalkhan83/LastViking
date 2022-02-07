using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using Extensions;
using Game.Models;
using static Game.Models.ObjectivesProgressModel;
using Game.Objectives.Stacks.Conditions.Base;

namespace Game.Objectives.Stacks
{
    [CreateAssetMenu(fileName = "ObjectivesStack", menuName = "UnityProject/ObjectivesStack", order = 0)]
    public partial class ObjectivesStack : ScriptableObject
    {
        #region Data
#pragma warning disable 0649

        [Header("Settings")]
        [SerializeField] private ObjectivesStackType stackType;
        [SerializeField] private ObjectivesType objectivesType;

        [ReorderableList] [HideIf("StackIsCompozite")] [SerializeField] private List<ObjectiveData> objectives;
        [ReorderableList] [ShowIf("StackIsCompozite")] [SerializeField] private List<ObjectivesStack> subStacks;
        [ShowIf("StackIsCompozite")] [SerializeField] private bool useItselfForReroll; // feature avaliable only for compozite stacks
        [ShowIf("StackIsConditional")] [SerializeField] private List<StackObjectiveConditions> conditionsPerObjective;

        [Space]
        [Header("Runtime")]
        [SerializeField] private List<OjbectiveStackData> datas;
        [SerializeField] private List<OjbectiveStackData> notCompleatedData;
        [SerializeField] private List<OjbectiveStackData> completedData;

        [ProgressBar("Completion", 1, ProgressBarColor.Blue)]
        [SerializeField] private float progress;

#pragma warning restore 0649
        #endregion

        #region Private properties
        private static ObjectivesProgressModel ObjectivesProgressModel { get => ModelsSystem.Instance._objectivesProgressModel; }

        private List<OjbectiveStackData> _Datas {get => datas; set => datas = value;}
        private ObjectivesStackType _StackType => stackType;
        private List<ObjectiveData> _Objectives => objectives;


        #endregion

        #region Public Properties
        public ObjectivesStackType StackType => stackType;
        public ObjectivesType ObjectiveType => objectivesType;
        public List<OjbectiveStackData> Datas => datas;
        public bool NotCompleated => notCompleatedData.Count != 0;

        public bool IsSelected { get; set; }
        #endregion


        #region Methods: change/Set data
        public void RegenerateStack() => GenerateData();
        [Button]
        private void GenerateData()
        {
            if (_StackType == ObjectivesStackType.Compozite)
            {
                subStacks.ForEach(x =>
                {
                    x.GenerateData();
                    x.Datas.ForEach(y => y.SetCompoziteStack(this));
                });

            }

            _Datas = GetDatas();

            Update();
        }


        private void Update() // private but used by sub class ObjectivesStackData
        {
            notCompleatedData.Clear();
            completedData.Clear();

            if (_StackType == ObjectivesStackType.Compozite)
                _Datas = GetDatas();


            notCompleatedData.AddRange(_Datas.Where(x => !x.Done));
            completedData.AddRange(_Datas.Where(x => x.Done));

            //TODO: write about limitations with one level of compozition
            var totalObjectives = _StackType == ObjectivesStackType.Compozite ? subStacks.SelectMany(x => x._Objectives).Count() : _Objectives.Count;

            progress = (float)completedData.Count / (float)totalObjectives;
        }

        public void UpdateConditions()
        {
            for (int i = 0; i < _Objectives.Count; i++)
            {
                var conditionsOk = GetConditionsPerObjective(i);
                _Datas[i].SetConditionsOk(conditionsOk);
            }
        }

        #endregion


        #region  Methods: get data
        public OjbectiveStackData GetSavedElement(StackProgress data)
        {
            switch (_StackType)
            {
                case ObjectivesStackType.Linear:
                    return GetSavedByName(data.selectedObjective);
                case ObjectivesStackType.Random:
                    return GetSavedByName(data.selectedObjective);
                case ObjectivesStackType.Compozite:
                    return GetSavedByName(data.selectedObjective);
                case ObjectivesStackType.Conditional:
                    return GetSavedByName(data.selectedObjective);
                default:
                    Debug.LogError("Error here");
                    return null;
            }
        }

        public OjbectiveStackData GetElement()
        {

            switch (_StackType)
            {
                case ObjectivesStackType.Linear:
                    return GetFirstNotCompleatedElement();
                case ObjectivesStackType.Random:
                    return GetRandomElement();
                case ObjectivesStackType.Compozite:
                    var notCompleatedStacks = subStacks.Where(x => x.NotCompleated).ToList();
                    if (notCompleatedStacks.Count == 0)
                    {
                        return null; // FIXME: fix it ?
                    }
                    else
                    {
                        var randomNotCompleatedStack = notCompleatedStacks.RandomElement(); // TODO: add test for sub stacks with substacks  check ?
                        var element = randomNotCompleatedStack.GetElement();
                        return element;
                    }
                case ObjectivesStackType.Conditional:
                    return GetFirstElementWithTrueConditions();
                default:
                    Debug.LogError("Error here");
                    return null;
            }
        }

        public OjbectiveStackData GetElement_LateGame()
        {

            switch (_StackType)
            {
                case ObjectivesStackType.Linear:
                    return GetLastElement();
                case ObjectivesStackType.Random:
                    return GetRandomElement();
                case ObjectivesStackType.Compozite:

                    var randomNotCompleatedStack = subStacks.RandomElement(); // TODO: add test for sub stacks with substacks  check ?
                    var element = randomNotCompleatedStack.GetLastElement();
                    return element;
                case ObjectivesStackType.Conditional:
                    return GetFirstElementWithTrueConditions();
                default:
                    Debug.LogError("Error here");
                    return null;
            }
        }

        private bool ElementIsValid(OjbectiveStackData data)
        {
            switch (_StackType)
            {
                case ObjectivesStackType.Linear:
                    return NotCompleatedElementIsValid(data);
                case ObjectivesStackType.Random:
                    return AnyElementIsValid(data);
                case ObjectivesStackType.Compozite:
                    return NotCompleatedElementIsValid(data);
                case ObjectivesStackType.Conditional:
                    return AnyElementIsValid(data);
                default:
                    Debug.LogError("Error here");
                    return false;
            }
        }

        private bool ElementIsRefreshable()
        {
            switch (_StackType)
            {
                case ObjectivesStackType.Conditional:
                    return true;
                default:
                    return false;
            }
        }

        private bool UseSameStackOnReroll()
        {
            switch (_StackType)
            {
                case ObjectivesStackType.Linear:
                    return false;
                case ObjectivesStackType.Random:
                    return false;
                case ObjectivesStackType.Compozite:
                    return useItselfForReroll;
                case ObjectivesStackType.Conditional:
                    return false;
                default:
                    Debug.LogError("Error here");
                    return false;
            }
        }

        private List<OjbectiveStackData> GetDatas()
        {
            var datas = new List<OjbectiveStackData>();

            switch (_StackType)
            {
                case ObjectivesStackType.Compozite:
                    datas = subStacks.SelectMany(x => x.Datas).ToList();
                    break;
                default:
                    for (int i = 0; i < _Objectives.Count; i++)
                    {
                        bool conditionsOk = GetConditionsPerObjective(i);
                        ObjectiveData objective = _Objectives[i];
                        bool done = GetIsDonePerObjective(conditionsOk, objective);


                        datas.Add(new OjbectiveStackData(objective, done, this, conditionsOk));
                    }
                    break;
            }
            return datas;
        }

        private bool GetIsDonePerObjective(bool conditionOk, ObjectiveData objectiveData)
        {
            switch (_StackType)
            {
                case ObjectivesStackType.Conditional:
                    return GetIsDonePerObjectiveConditional(conditionOk, objectiveData);
                default:
                    return GetIsDonePerObjectiveDefault(objectiveData);
            }
        }

        #endregion


        #region Help Methods

        private bool GetConditionsPerObjective(int index)
        {
            if (!StackIsConditional()) return true;
            if (conditionsPerObjective.IndexOutOfRange(index)) return true;

            bool conditionsOk = conditionsPerObjective[index].ConditionsOK();
            return conditionsOk;
        }

        // used by [HideIf] and [ShowIf] attributes
        private bool StackIsCompozite() => _StackType == ObjectivesStackType.Compozite;
        private bool StackIsConditional() => _StackType == ObjectivesStackType.Conditional;

        #endregion
    }
}