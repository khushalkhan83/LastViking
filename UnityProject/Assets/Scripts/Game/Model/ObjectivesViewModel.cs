using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class ObjectivesViewModel : InitableModel<ObjectivesViewModel.Data>
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredBool IsHas1;
            public ObscuredBool IsHas2;
            public ObscuredBool IsHas3;
            public ObscuredBool IsHas4;
            public ObscuredBool IsHas5;
            public ObscuredBool IsHas6;

            public void SetIsHas1(bool value)
            {
                if (IsHas1 != value)
                {
                    IsHas1 = value;
                    ChangeData();
                }
            }

            public void SetIsHas2(bool value)
            {
                if (IsHas2 != value)
                {
                    IsHas2 = value;
                    ChangeData();
                }
            }

            public void SetIsHas3(bool value)
            {
                if (IsHas3 != value)
                {
                    IsHas3 = value;
                    ChangeData();
                }
            }

            public void SetIsHas4(bool value)
            {
                if (IsHas4 != value)
                {
                    IsHas4 = value;
                    ChangeData();
                }
            }

            public void SetIsHas5(bool value)
            {
                if (IsHas5 != value)
                {
                    IsHas5 = value;
                    ChangeData();
                }
            }

            public void SetIsHas6(bool value)
            {
                if (IsHas6 != value)
                {
                    IsHas6 = value;
                    ChangeData();
                }
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        #region Reroll Animation
        private List<int> indexesToPlayRerollAnimations = new List<int>();

        public bool BlockPlayAnimation {get;set;}

        public bool NeedToPlayRerrollAnimation {get => indexesToPlayRerollAnimations.Count == 0;}

        public void SetAnimationMustPlay(int index)
        {
            if(indexesToPlayRerollAnimations.Contains(index)) return;
            indexesToPlayRerollAnimations.Add(index);
        }
        public bool GetAnimationMustPlay(int index) => BlockPlayAnimation == false && indexesToPlayRerollAnimations.Contains(index); 

        public void SetAnimationPlayed(int index)
        {
            indexesToPlayRerollAnimations.Remove(index);
        }

        #endregion


        public bool IsHas1
        {
            get => _data.IsHas1;
            set => _data.SetIsHas1(value);
        }

        public bool IsHas2
        {
            get => _data.IsHas2;
            set => _data.SetIsHas2(value);
        }

        public bool IsHas3
        {
            get => _data.IsHas3;
            set => _data.SetIsHas3(value);
        }

        public bool IsHas4
        {
            get => _data.IsHas4;
            set => _data.SetIsHas4(value);
        }

        public bool IsHas5
        {
            get => _data.IsHas5;
            set => _data.SetIsHas5(value);
        }

        public bool IsHas6
        {
            get => _data.IsHas6;
            set => _data.SetIsHas6(value);
        }

        public bool IsHasAny => IsHas1 || IsHas2 || IsHas3 || IsHas4 || IsHas5 || IsHas6;

        public StorageModel StorageModel => _storageModel;

        public bool IsOpenGlobalObjectives { get; private set; }

        protected override Data DataBase => _data;

        public event Action OnPickUpTutorialStep1;
        public event Action OnPickUpTutorialStep2;
        public event Action OnPickUpTutorialStep3;
        public event Action OnPickUpTutorialStep4;
        public event Action OnPickUpTutorialStep5;
        public event Action OnPickUpTutorialStep6;
        public event Action OnPickUpAnyTutorialStep;
        public event Action OnChangeIsOpenGlobalObjectives;
        public event Action OnShowTutorialButtonsOnTop;

        public void ChangeIsOpenGlobalObjectives()
        {
            IsOpenGlobalObjectives = !IsOpenGlobalObjectives;
            OnChangeIsOpenGlobalObjectives?.Invoke();
        }

        public void CloseGlobalObjectives()
        {
            if (IsOpenGlobalObjectives)
            {
                IsOpenGlobalObjectives = false;
                OnChangeIsOpenGlobalObjectives?.Invoke();
            }
        }

        public void PickUpTutorialStep1()
        {
            if (IsHas1)
            {
                IsHas1 = false;
                OnPickUpAnyTutorialStep?.Invoke();
                OnPickUpTutorialStep1?.Invoke();
            }
        }

        public void PickUpTutorialStep2()
        {
            if (IsHas2)
            {
                IsHas2 = false;
                OnPickUpAnyTutorialStep?.Invoke();
                OnPickUpTutorialStep2?.Invoke();
            }
        }

        public void PickUpTutorialStep3()
        {
            if (IsHas3)
            {
                IsHas3 = false;
                OnPickUpAnyTutorialStep?.Invoke();
                OnPickUpTutorialStep3?.Invoke();
            }
        }

        public void PickUpTutorialStep4()
        {
            if (IsHas4)
            {
                IsHas4 = false;
                OnPickUpAnyTutorialStep?.Invoke();
                OnPickUpTutorialStep4?.Invoke();
            }
        }

        public void PickUpTutorialStep5()
        {
            if (IsHas5)
            {
                IsHas5 = false;
                OnPickUpAnyTutorialStep?.Invoke();
                OnPickUpTutorialStep5?.Invoke();
            }
        }

        public void PickUpTutorialStep6()
        {
            if (IsHas6)
            {
                IsHas6 = false;
                OnPickUpAnyTutorialStep?.Invoke();
                OnPickUpTutorialStep6?.Invoke();
            }
        }

        public void ShowTutorialButtonsOnTop()
        {
            OnShowTutorialButtonsOnTop?.Invoke();
        }

        public event Action OnRedirectedFromPushNotification;
        
        public void RedirectedFromPushNotification()
        {
            OnRedirectedFromPushNotification?.Invoke();
        }
    }
}
