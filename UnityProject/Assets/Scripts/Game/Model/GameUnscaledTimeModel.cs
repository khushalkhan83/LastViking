using System;
using UnityEngine;

namespace Game.Models
{
    public class GameUnscaledTimeModel : MonoBehaviour
    {
        private float maxDeltaTime = 1f;

        public float DeltaTime
        {
            get
            {
                if (Time.unscaledDeltaTime >= maxDeltaTime)
                    return maxDeltaTime;
                else
                    return Time.unscaledDeltaTime;
            }
        }
    }
}
