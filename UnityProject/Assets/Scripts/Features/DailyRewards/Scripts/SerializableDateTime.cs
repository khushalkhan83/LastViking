using System;
using UnityEngine;

// TODO: move to common folder
namespace Game.Features
{
    [Serializable]
    public class SerializableDateTime
    {
        // Time
        [SerializeField] private int hour;
        [SerializeField] private int minute;
        [SerializeField] private int second;

        // Date
        [SerializeField] private int day;
        [SerializeField] private int month;
        [SerializeField] private int year;

        private DateTime _validatedDateTime => VaildateDateTime();
        public DateTime DateTime => _validatedDateTime;

        public SerializableDateTime(DateTime dateTime)
        {
            hour = dateTime.Hour;
            minute = dateTime.Minute;
            second = dateTime.Second;
            day = dateTime.Day;
            month = dateTime.Month;
            year = dateTime.Year;
        }

        private DateTime VaildateDateTime()
        {
            year = Mathf.Clamp(year, 1, 2100);
            month = Mathf.Clamp(month, 1, 12);
            day = Mathf.Clamp(day, 1, 31);
            hour = Mathf.Clamp(hour, 0, 23);
            minute = Mathf.Clamp(minute, 0, 59);
            second = Mathf.Clamp(second, 0, 59);

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}