using System;
using UnityEngine;

namespace Encounters
{
    public interface IEncounter
    {
        void Init(Vector3 spawnPoint);
        bool CanOccure {get;}
        float chanceWeight {get;}
        float cooldown {get;}
        bool IsActive{get;}
        bool isCooldown {get;}
        bool IsUnlocked {get;}
        void AddActionOnDeactivate(Action action);
        void DeInit();
        void Compleate();
    }
}