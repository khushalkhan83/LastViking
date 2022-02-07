using System;
using UnityEngine;

namespace Game.Models
{
    public class AutosaveModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private bool _saveOnEvents = true;

        [Space]
        [SerializeField] private float _initSaveDelay;
        [SerializeField] private int[] _saveHours;
        [SerializeField] private float _saveInterval = 60;

#pragma warning restore 0649
        #endregion

        public DateTime LastSaveTime { get; set; }

        public event Action OnSave;

        public bool SaveOnEvents => _saveOnEvents;
        public float InitSaveDelay => _initSaveDelay;
        public int[] SaveHours => _saveHours;
        public float SaveInterval => _saveInterval;

        public void Save()
        {
            LastSaveTime = DateTime.Now;
            OnSave?.Invoke();
        }

    }
}
