using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerHealthRegenerationModel : MonoBehaviour
    {
        [SerializeField] private float regenInterval = 20f;

        public float RegenInterval => regenInterval;

        public float RegenValue {get; private set;}

        public void SetRegenValue(float value)
        {
            RegenValue = value;
        }
    }
}
