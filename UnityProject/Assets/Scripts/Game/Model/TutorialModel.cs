using Core.Storage;
using Game.Controllers.Controllers.States.Modificators;
using Game.Objectives;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Game.Models
{
    public class TutorialModel : MonoBehaviour
    {
        [Serializable]
        public class TutorialModelData : DataBase, IImmortal
        {
            public bool IsComplete;
            public bool IsStart;
            public bool IsHasObjectiveId;
            public bool ResetedToConstruction;
            public bool IsPostTutorialCompleated;
            public ushort ObjectiveId;
            public int Step;
            [SerializeField] private bool isActive;

            public void SetIsComplete(bool isComplete)
            {
                IsComplete = isComplete;
                ChangeData();
            }

            public void SetIsStart(bool isStart)
            {
                IsStart = isStart;
                ChangeData();
            }

            public void SetIsHasObjectiveId(bool isHasObjectiveId)
            {
                IsHasObjectiveId = isHasObjectiveId;
                ChangeData();
            }

            public void SetResetedToConstruction(bool resetedToConstruction)
            {
                ResetedToConstruction = resetedToConstruction;
                ChangeData();
            }
            public void SetIsPostTutorialCompleated(bool isPostTutorialCompleated)
            {
                IsPostTutorialCompleated = isPostTutorialCompleated;
                ChangeData();
            }

            public void SetObjectiveId(ushort objectiveId)
            {
                ObjectiveId = objectiveId;
                ChangeData();
            }

            public void SetStep(int step)
            {
                Step = step;
                ChangeData();
            }
            public bool IsActive { get { return isActive; } set { isActive = value; ChangeData(); } }
        }

        
        private const int StepsCount = 13;

        public void SetNextStep()
        {
            if(Step + 1 < StepsCount)
                SetStep(Step + 1);
            else
                CompleteTuturial();
        }

        public void SetStep(int step)
        {
            Step = step;
            OnStepChanged?.Invoke();
        }

        public event Action OnStepChanged;
        public event Action OnIsActiveChanged;

        public bool IsActive
        {
            get => _data.IsActive;
            private set => _data.IsActive = value;
        }

        public void ActivateTutorial()
        {       
            IsActive = true;    
            OnIsActiveChanged?.Invoke();
        }

        public void CompleteTuturial()
        {
            IsActive = false;
            IsComplete = true;
            OnIsActiveChanged?.Invoke();

            Debug.Log("Compleated");
        }


        #region Data
#pragma warning disable 0649

        [SerializeField] private TutorialModelData _data;
        [SerializeField] int constructionFirstStep;
        [SerializeField] int buildFoundationStep;
        [SerializeField] int buildWallsStep;
        [SerializeField] int buildRoofStep;
        [SerializeField] int buildDoorStep;
        [SerializeField] ObjectiveID[] _objectiveIDs;
        [SerializeField] StorageModel _storageModel;
        [SerializeField] private TutorialModificator startModificator;
        [SerializeField] private TutorialModificator startPostTutorialModificator;
        [SerializeField] private TutorialModificator postTutorialModificator;
        [SerializeField] private TutorialModificator[] tutorialModificators;

#pragma warning restore 0649
        #endregion

        #if UNITY_EDITOR
        [SerializeField] private int testTutorStep;

        [Button]
        private void Test_SetStep() => SetStep(testTutorStep);
        #endif

        public int ConstructionFirstStep => constructionFirstStep;
        public int BuildFoundationStep => buildFoundationStep;
        public int BuildWallsStep => buildWallsStep;
        public int BuildRoofStep => buildRoofStep;
        public int BuildDoorStep => buildDoorStep;
        public ObjectiveID[] ObjectiveIDs => _objectiveIDs;
        public StorageModel StorageModel => _storageModel;

        public int Step
        {
            get => _data.Step;
            private set => _data.SetStep(value);
        }

        public ushort ObjectiveId
        {
            get => _data.ObjectiveId;
            private set => _data.SetObjectiveId(value);
        }

        public bool IsHasObjectiveId
        {
            get => _data.IsHasObjectiveId;
            private set => _data.SetIsHasObjectiveId(value);
        }

        public bool IsComplete
        {
            get => _data.IsComplete;
            private set => _data.SetIsComplete(value);
        }

        public bool IsStart
        {
            get => _data.IsStart;
            private set => _data.SetIsStart(value);
        }

        public bool RestededToConstruction
        {
            get => _data.ResetedToConstruction;
            set => _data.SetResetedToConstruction(value);
        }

        public bool IsPostTutorialCompleated
        {
            get => _data.IsPostTutorialCompleated;
            set => _data.SetIsPostTutorialCompleated(value);
        }

        public TutorialModificator StartModificator => startModificator;
        public TutorialModificator StartPostTutorialModificator => startPostTutorialModificator;
        public TutorialModificator PostTutorialModificator => postTutorialModificator;

        public TutorialModificator Modificator
        {
            get
            {
                if(IsComplete) return NoModificator;
                
                var modificator = tutorialModificators[Step];
                return modificator;
            }
        }

        public TutorialModificator NoModificator
        {
            get
            {
                return new TutorialModificator();
            }
        }

        public event Action OnSkipTutorial;
        public event Action OnStart;
        public event Action OnComplete;
        public event Action OnNextStep;
        public event Action OnEndStep;

        public bool IsTutorialNow => IsStart && !IsComplete;
        public ObjectiveID ObjectiveIDCurrent => ObjectiveIDs[Step];
        public int StepLast => ObjectiveIDs.Length - 1;

        private bool datatInited = false;

        // called at PreInitState
        internal void Init()
        {
            if(datatInited) return;

            StorageModel.TryProcessing(_data);
            datatInited = true;
        }

        public void OnEnable()
        {
            Init();
        }

        public void SkipTutorial()
        {
            OnSkipTutorial?.Invoke();
            EndStepTo(StepLast + 1);
        }

        public void Save(ushort id)
        {
            IsHasObjectiveId = true;
            ObjectiveId = id;
        }

        public void EndStep() => EndStepTo(Step + 1);

        private void EndStepTo(int step)
        {
            OnEndStep?.Invoke();

            Step = step;
            IsHasObjectiveId = false;
            ObjectiveId = 0;

            if (Step >= StepLast + 1)
            {
                IsComplete = true;
                OnComplete?.Invoke();
            }
            else
            {
                OnNextStep?.Invoke();
            }
        }

        public void StartTutorial()
        {
            IsStart = true;
            OnStart?.Invoke();
        }

        public void Complete()
        {
            IsComplete = true;
            OnComplete?.Invoke();
        }

        public void ResetTutorial()
        {
            _data.SetIsComplete(false);
            _data.SetIsStart(true);
            _data.SetStep(0);
            _data.SetIsHasObjectiveId(false);
            _data.SetObjectiveId(0);
        }

        public void ResetToConstrution()
        {
            _data.SetIsComplete(false);
            _data.SetIsStart(true);
            _data.SetStep(ConstructionFirstStep);
            _data.SetIsHasObjectiveId(false);
            _data.SetObjectiveId(0);
            _data.SetResetedToConstruction(true);
        }
    }
}
