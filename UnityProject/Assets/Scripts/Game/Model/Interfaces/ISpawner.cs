using System;

namespace Game.Models
{
    public interface ISpawner
    {
        event Action OnSpawned;
    }
}