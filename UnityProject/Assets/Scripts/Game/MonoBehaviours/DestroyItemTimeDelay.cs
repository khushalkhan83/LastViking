using Core.Storage;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItemTimeDelay : MonoBehaviour, IData
{
    [Serializable]
    public class Data : DataBase
    {
        public float WaitingTime;

        public void SetWaitingTime(float waitingTime)
        {
            WaitingTime = waitingTime;
            ChangeData();
        }

        public bool ComponentRemoved;
        public void SetComponentRemoved(bool value)
        {
            ComponentRemoved = value;
            ChangeData();
        }
    }

    #region Data
#pragma warning disable 0649

    [SerializeField] private Data _data;
    [SerializeField] private WorldObjectModel _worldObjectModel;
    [SerializeField] private float _destroyTime;

    public event Action OnDataInitialize;

#pragma warning restore 0649
    #endregion

    public float WaitingTime
    {
        get => _data.WaitingTime;
        private set => _data.SetWaitingTime(value);
    }

    public bool ComponentRemoved
    {
        get => _data.ComponentRemoved;
        private set => _data.SetComponentRemoved(value);
    }

    public float DestroyTime => _destroyTime;

    private WorldObjectModel WorldObjectModel => _worldObjectModel;
    private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

    private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

    public IEnumerable<IUnique> Uniques
    {
        get
        {
            yield return _data;
        }
    }

    private void OnEnable()
    {
        GameUpdateModel.OnUpdate += OnUpdateHandler;
    }

    private void Start()
    {
        
    }

    public void UUIDInitialize()
    {
        StorageModel.TryProcessing(_data);
        if(_data.ComponentRemoved == true)
        {
            Destroy(this);
        }

        OnDataInitialize?.Invoke();
    }

    private void OnDisable()
    {
        GameUpdateModel.OnUpdate -= OnUpdateHandler;
    }

    private void OnUpdateHandler()
    {
        WaitingTime -= Time.deltaTime;
        if (WaitingTime <= 0)
        {
            GameUpdateModel.OnUpdate -= OnUpdateHandler;
            DestroyItem();
            WaitingTime = _destroyTime;
        }
    }

    private void DestroyItem()
    {
        WorldObjectModel.Delete();
        Destroy(this.gameObject);
    }

    public void RemoveComponentAndSaveIt()
    {
        ComponentRemoved = true;
        Destroy(this);
    }
}
