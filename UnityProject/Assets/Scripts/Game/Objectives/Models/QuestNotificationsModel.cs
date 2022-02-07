using System;
using UnityEngine;

namespace Game.Models
{
    public class QuestNotificationsModel : MonoBehaviour
    {
        public enum State
        {
            Default,
            TargetHealth,
            ItemsCount
        }

        public string Message {get; private set;}
        public Sprite Icon {get; private set;}
        public IHealth TargetEnemyHealth {get; private set;}
        public bool IsShown {get; private set;}

        public int TargetItemsCount {get; private set;}
        public int HaveItemsCount {get; private set;}

        public State NotificationType {get; private set;} = State.Default;

        public event Action OnShow;
        public event Action OnSetTargetEnemyHealth;
        public event Action OnSetTargetItemsCount;
        public event Action OnClearTargetEnemyHealth;
        public event Action OnHide;

        public void Show(string message, Sprite icon)
        {
            Message = message;
            Icon = icon;
            IsShown = true;
            OnShow?.Invoke();

            if(TargetEnemyHealth != null && !TargetEnemyHealth.IsDead)
            {
                var target = TargetEnemyHealth;
                RemoveTargetEnemyHealth();
                SetTargetEnemyHealth(target);
            }
        }

        public void SetTargetEnemyHealth(IHealth health)
        {
            NotificationType = State.TargetHealth;
            TargetEnemyHealth = health;
            OnSetTargetEnemyHealth?.Invoke();
        }

        public void RemoveTargetEnemyHealth()
        {
            NotificationType = State.Default;
            OnClearTargetEnemyHealth?.Invoke();
            TargetEnemyHealth = null;
        }

        public void SetTargetItemsCount(int have, int target)
        {
            NotificationType = State.ItemsCount;
            HaveItemsCount = have;
            TargetItemsCount = target;
            OnSetTargetItemsCount?.Invoke();
        }

        public void SetDefaultState()
        {
            NotificationType = State.Default;
        }

        public void Hide()
        {
            IsShown = false;
            OnHide?.Invoke();
        }
    }
}
