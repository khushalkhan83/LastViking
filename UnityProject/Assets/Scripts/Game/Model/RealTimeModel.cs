using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

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

        public bool InterntedTimeSetedOnce {get; private set;}

        public event Action OnTimeReady;
        public event Action OnTimeReset;
        public event Action<string> OnTimeError;

        public DateTime Now()
        {
            if (isReady && TryCalculate_NowTime(out var answer))
            {
                return answer;
            }

            return DateTime.MinValue;
        }

        // TODO: refactor
        public bool TryCalculate_NowTime(out DateTime answer)
        {
            answer = DateTime.MinValue;
            if(!InterntedTimeSetedOnce) return false;
            
            DateTime loc = DateTime.Now;
            TimeSpan ts = loc.Subtract(_onSyncLocalTime);
            ts = GetLimitedTimeSpan(ts);
            answer = _onlineDateTime.Add(ts);
            return true;
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
            InterntedTimeSetedOnce = true;
            OnTimeReady?.Invoke();
        }

        // TODO: Instead of this cheat detection use ApplicationCallbacksModel and update time on application changed
        private TimeSpan GetLimitedTimeSpan(TimeSpan ts)
        {
            TimeSpan timeSpanLimit = new TimeSpan(2,0,0);
            return ts > timeSpanLimit ? timeSpanLimit : ts;
        }
    }
}