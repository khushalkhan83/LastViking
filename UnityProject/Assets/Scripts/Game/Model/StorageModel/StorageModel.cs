using Core.Storage;
using Game.Storage;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace Game.Models
{
    public class StorageModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

#pragma warning restore 0649
        #endregion

#if UNITY_EDITOR
        public bool IsRecord {get => EditorGameSettings.Instance.saveProgress;}
#endif
        private IStorage Storage { get; } 
#if UNITY_EDITOR
            = new StorageEditor();
#else
            = new StorageMobile();
#endif
        public string SaveRootPath => (Storage as StorageFiles)?.RootPath;
        private HashSet<IUnique> _tracked = new HashSet<IUnique>();
        private HashSet<IUnique> _trackedImmortal = new HashSet<IUnique>();
        private HashSet<IUnique> _changed = new HashSet<IUnique>();
        private HashSet<IUnique> _lateSavable = new HashSet<IUnique>();
        private HashSet<string> _pendingToClear = new HashSet<string>();

        public event Action OnPreSaveAll;
        public event Action OnPreSaveChanged;
        public event Action OnPackData;

        public event Action OnSaveAll;
        public event Action OnSaveChanged;

        public bool IsSavingAll { get; private set; }
        public bool IsSavingChanged { get; private set; }
        public bool IsSavingLate { get; private set; }

        public bool TryLoad<T>(T data) where T : class, IUnique
        {
            if (IsHasSave(data))
            {
                try
                {
                    Storage.Load(data);
                }
                catch
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        public void Tracking<T>(T data) where T : class, IUnique
        {
#if UNITY_EDITOR
            if (!IsRecord)
            {
                return;
            }
#endif
            if (data is IImmortal)
            {
                AddTrackedImmortal(data);
            }
            else
            {
                AddTracked(data);
            }

            SubsribeOnDataChange(data);
        }

        private void OnDataChanged<T>(T data, DataBase.SaveTime saveTime) where T : class, IUnique
        {
            if (saveTime == DataBase.SaveTime.None)
            {
                throw new Exception();
            }

            if (saveTime.HasFlag(DataBase.SaveTime.Instantly))
            {
                Save(data);
            }
            if (saveTime.HasFlag(DataBase.SaveTime.Deffered))
            {
                UnsubsribeOnDataChange(data);
                AddChanged(data);
            }
            if (saveTime.HasFlag(DataBase.SaveTime.ChangeContext))
            {
                UnsubsribeOnDataChange(data);
                AddLateSavable(data);
            }
        }

        public bool IsHasSave<T>(T data) where T : class, IUnique => Storage.IsHasSave(data);

        private void AddTrackedImmortal<T>(T data) where T : class, IUnique => AddToTrackedCollection(data, _trackedImmortal);

        private void AddTracked<T>(T data) where T : class, IUnique => AddToTrackedCollection(data, _tracked);

        private void AddChanged<T>(T data) where T : class, IUnique => AddToTrackedCollection(data, _changed);

        private void AddLateSavable<T>(T data) where T : class, IUnique => AddToTrackedCollection(data, _lateSavable);

        private void AddToTrackedCollection<T>(T data, ICollection<IUnique> trackCollections) where T : class, IUnique
        {
            if (!trackCollections.Contains(data))
            {
                trackCollections.Add(data);
            }
        }

        private void SubsribeOnDataChange<T>(T data) where T : class, IUnique
        {
            if (data is DataBase db)
            {
                db.OnDataChanged += OnDataChanged;
            }
        }
        private void UnsubsribeOnDataChange<T>(T data) where T : class, IUnique
        {
            if (data is DataBase db)
            {
                db.OnDataChanged -= OnDataChanged;
            }
        }

        public void Untracking<T>(T data) where T : class, IUnique
        {
            _tracked.Remove(data);
            UnsubsribeOnDataChange(data);
            _changed.Remove(data);
            _lateSavable.Remove(data);
        }

        public bool TryProcessing<T>(T data) where T : class, IUnique
        {
            Tracking(data);
            return TryLoad(data);
        }

        public bool TryProcessing<T>(IEnumerable<T> datas) where T : class, IUnique
        {
            var result = true;
            foreach (var data in datas)
            {
                Tracking(data);
                result &= TryLoad(data);
            }
            return result;
        }

        public void ClearTracked()
        {
            foreach (var data in _tracked)
            {
                Storage.Clear(data);
            }
            foreach (var data in Tracked())
            {
                UnsubsribeOnDataChange(data);
            }
            _tracked.Clear();
        }

        public void ClearChanged()
        {
            _changed.Clear();
        }

        public void ClearLateSavable()
        {
            _lateSavable.Clear();
        }

        public void ClearPendingToClear()
        {
            foreach(string uuid in _pendingToClear)
            {
                try
                {   
                    Storage.ClearByUUID(uuid);
                }
                catch(Exception ex)
                {
                    Debug.LogException(new Exception($"Can`t clear uuid: {uuid} {ex.ToString()}"));
                }
            }
            _pendingToClear.Clear();
        }

        public void ClearAll()
        {
            Storage.ClearAll();
            _tracked.Clear();
            _changed.Clear();
            _lateSavable.Clear();
            _pendingToClear.Clear();
        }

        public void ClearAllWithImmortal()
        {
            ClearAll();
            _trackedImmortal.Clear();
        }

        public void Save<T>(T data) where T : IUnique
        {
            try
            {
                Storage.Save(data);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(new Exception($"Can`t save changes: {ex.ToString()}"));
                throw ex;
            }
        }

        public void SaveAll()
        {
            if (IsSavingAll) return;
            IsSavingAll = true;

            try
            {
                OnPackData?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(new Exception($"Error in OnPackData: {ex.ToString()}"));
            }
            
            try
            {
                OnPreSaveAll?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(new Exception($"Error in OnPreSaveAll:{ex.ToString()}"));
            }

            foreach (var data in AllData())
            {
                try
                {
                    Storage.Save(data);
                }
                catch (System.Exception ex)
                {
                    var message = $"{data.UUID} cant save";
                    message.Error();
                    Debug.LogException(new Exception($"Save all data: {message} {ex.ToString()}"));
                }
            }

            try
            {
                OnSaveAll?.Invoke();
            }
            catch(System.Exception ex)
            {
                Debug.LogException(new Exception($"Error in OnSaveAll: {ex.ToString()}"));
            }
            IsSavingAll = false;
        }

        private List<IUnique> AllData() => _tracked.Union(_trackedImmortal).ToList();

        public void SaveChanged()
        {
            if (IsSavingChanged || IsSavingAll) return;
            IsSavingChanged = true;

            OnPackData?.Invoke();
            OnPreSaveChanged?.Invoke();

            foreach (var data in _changed.ToList())
            {
                SubsribeOnDataChange(data);
                Storage.Save(data);
            }

            ClearChanged();
            ClearPendingToClear();
            OnSaveChanged?.Invoke();

            IsSavingChanged = false;
        }

        public void SaveLateSavable()
        {
            if (IsSavingLate || IsSavingAll) return;
            IsSavingLate = true;

            foreach (var data in _lateSavable.ToList())
            {
                SubsribeOnDataChange(data);
                Storage.Save(data);
            }

            ClearLateSavable();

            IsSavingLate = false;
        }

        public void Clear<T>(T data) where T : IUnique => _pendingToClear.Add(data.UUID);

        private IEnumerable<IUnique> Tracked()
        {
            foreach (var item in _tracked)
            {
                yield return item;
            }
            foreach (var item in _trackedImmortal)
            {
                yield return item;
            }
        }

        private IEnumerable<IUnique> Changed()
        {
            foreach (var item in _changed)
            {
                yield return item;
            }
        }

        private IEnumerable<IUnique> LateSavable()
        {
            foreach (var item in _lateSavable)
            {
                yield return item;
            }
        }
    }
}
