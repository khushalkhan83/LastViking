using UnityEngine;
using System;

namespace Updaters
{
    public class DelayedUpdater : IUpdater
    {
        private float timeToGo;
        private readonly Action onUpdate;

        public float UpdateRate { get; }

        public DelayedUpdater(float updateRate, Action OnUpdate, float? initalDelay = null)
        {
            UpdateRate = updateRate;
            onUpdate = OnUpdate;

            if(initalDelay.HasValue)
            {
                SetNextTime(initalDelay.Value);
            }
        }

        public void Tick()
        {
            if (Time.fixedTime >= timeToGo)
            {
                onUpdate?.Invoke();
                SetNextTime(UpdateRate);
            }
        }

        private void SetNextTime(float wait)
        {
            timeToGo = Time.fixedTime + wait;
        }
    }
}