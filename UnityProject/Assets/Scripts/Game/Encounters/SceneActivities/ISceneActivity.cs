using System;
using UnityEngine;

namespace Encounters
{
    public interface ISceneActivity
    {
        void Spawn(Vector3 spawnPoint);
        void Despawn();
        event Action OnCompleated;
        event Action OnFarAwayDespawned;
        Vector3 position {get;}
    }
}