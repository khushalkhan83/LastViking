using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActivityLog.Data
{
    public interface IActivityLogEnterenceProducer
    {
        bool TryGetActivityLogEnterence(out ActivityLogEnterenceData activityLogEnterence);
    }

    public class ActivityLogEnterenceData
    {
        public ActivityLogEnterenceData(Func<string> message, Sprite icon)
        {
            Message = message;
            Icon = icon;
        }

        public Func<string> Message { get; }
        public Sprite Icon { get; }
    }

    public interface IActivityLogEnterencesModel
    {
        List<ActivityLogEnterenceData> GetActivitiesEnterences();
        event Action OnActivitiesCountChanged;
    }
}