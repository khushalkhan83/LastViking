using System;

namespace Game.Models
{
    public interface IHealth
    {
        bool IsDead { get; }
        float Health { get; }
        float HealthMax { get; }
        void AdjustHealth(float adjustment);
        void SetHealth(float health);
        event Action OnChangeHealth;
        event Action OnDeath;
    }
}
