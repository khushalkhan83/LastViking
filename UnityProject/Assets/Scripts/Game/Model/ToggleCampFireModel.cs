using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public interface ISimpleInteractable
    {
        void Interact();
        event Action OnInteract;
    }

    public interface IToggleInteractable : ISimpleInteractable
    {
        bool On { get; }
    }

    public class ToggleCampFireModel : MonoBehaviour, IToggleInteractable
    {

        [System.Serializable]
        public class Data : DataBase
        {
            public bool On;
            public bool WasSpawned;
            public ulong NextSpawnBlueprintsTicks;

            public void SetOn(bool on)
            {
                On = on;
                ChangeData();
            }
            public void SetWasSpawned(bool wasSpawned)
            {
                WasSpawned = wasSpawned;
                ChangeData();
            }

            public void SetNextSpawnBlueprintsTicks(ulong nextSpawnBlueprintsTicks)
            {
                NextSpawnBlueprintsTicks = nextSpawnBlueprintsTicks;
                ChangeData();
            }
        }

        [SerializeField] private Data _data;

        // cant name this Data
        public Data _Data => _data;

        public event Action OnInteract;

        public bool On
        {
            get => _data.On;
            set => _data.SetOn(value);
        }
        public bool WasSpawned
        {
            get => _data.WasSpawned;
            set => _data.SetWasSpawned(value);
        }



        public void Interact()
        {
            On = !On;
            OnInteract?.Invoke();
        }
    }
}
