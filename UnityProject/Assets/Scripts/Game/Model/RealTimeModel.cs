using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Models
{
    public class RealTimeModel : MonoBehaviour
    {
        [Serializable]
        public class TimeJSON
        {
            public string UTC;
        }

        enum Status { disconnected, connecting, ready }

        [SerializeField] string kURL = "";
        public string URL { get { return kURL; } }

        Status status = Status.disconnected;

        public bool isReady { get { return status == Status.ready; } }

        DateTime _onlineDateTime;
        DateTime _onSyncLocalTime;

        public event Action OnTimeReady;
        public event Action OnTimeReset;
        public event Action<string> OnTimeError;

        public DateTime Now()
        {
            if (isReady)
            {
                DateTime loc = DateTime.Now;
                TimeSpan ts = loc.Subtract(_onSyncLocalTime);
                return _onlineDateTime.Add(ts);
            }

            return DateTime.MinValue;
        }

        public void DropTime()
        {
            status = Status.disconnected;
            OnTimeReset?.Invoke();
        }

        public void UpdateTimeError(string error)
        {
            Debug.LogError(error);
            status = Status.disconnected;
            OnTimeError?.Invoke(error);
        }

        public void SetInternetTime(DateTime now, DateTime checkTime)
        {
            _onlineDateTime = now;
            _onSyncLocalTime = checkTime;
            status = Status.ready;
            OnTimeReady?.Invoke();
        }
    }
}