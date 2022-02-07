using Core.Storage;
using Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Game.Progressables;

namespace Game.Interactables
{
    /* Сделать разные классы: Savable..., NonSavable... 
     * Сделать в Activatable метод OnReactivate ? / OnActivateInstantly
     * Сделать конфиг, в котором будет Активатабл и IsActivate повторно    
    */

    public class ObjectsActivator : MonoBehaviour, IProgressable
    {
        [Serializable]
        public class Data : DataBase
        {
            public ProgressStatus ProgressStatus;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetProgressStatus(ProgressStatus status)
            {
                ProgressStatus = status;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private bool _shouldSaveProgress = true;
        [SerializeField] private Activatable[] _activatables;
        [SerializeField] private bool useReactivate;
        [EnableIf("useReactivate")] [SerializeField] private Activatable[] _reActivatables;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _triggerName = "Activate";
        [SerializeField] private GameObject[] _objestsToDisable;
 
#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        public bool ShouldSaveProgress => _shouldSaveProgress;
        public Activatable[] Activatables => _activatables;
        public Activatable[] ReActivatables => _reActivatables;
        public Animator Animator => _animator;
        public GameObject[] ObjestsToDisable => _objestsToDisable;
        public int TriggerHash => Animator.StringToHash(_triggerName);


        #region IProgressable
        public ProgressStatus ProgressStatus
        {
            get => _data.ProgressStatus;
            set => _data.SetProgressStatus(value);
        }

        public void ClearProgress() { }
            
        #endregion

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private WaterfallProgressModel WaterfallProgressModel => ModelsSystem.Instance._waterfallProgressModel;

        #region MonoBehaviour
        private void OnEnable() => StorageModel.TryProcessing(_data);
        private void OnDisable() => StorageModel.Untracking(_data);

        private IEnumerator Start()
        {
            yield return null;
            if (ShouldSaveProgress && ProgressStatus == ProgressStatus.InProgress)
            {
                Activate(reactivate: true);
            }
        }
        #endregion


        public void Activate(bool reactivate = false)
        {
            if (ShouldSaveProgress)
                ProgressStatus = ProgressStatus.InProgress;

            var collection = reactivate && useReactivate ? ReActivatables: Activatables;

            foreach (var act in collection)
                act.OnActivate();
            if (Animator)
                Animator.SetTrigger(TriggerHash);
            foreach (var o in ObjestsToDisable) o.SetActive(false);
        }
    }
}
