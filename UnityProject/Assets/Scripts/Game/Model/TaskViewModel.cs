using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public class TaskViewModel : MonoBehaviour
    {
        public bool IsShow { get; private set; }
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public event Action OnMessageChanged;
        public event Action OnCountFieldChanged;
        public event Action OnShowFillChanged;
        public event Action OnFillAmountChanged;
        public event Action OnIconChanged;

        public string Message {get; private set;}
        public string Count {get; private set;}
        public bool ShowFill {get; private set;}
        public float FillAmount {get; private set;}
        public Sprite Icon {get; private set;}

        public void SetMessage(string message)
        {
            Message = message;
            OnMessageChanged?.Invoke();
        }

        public void SetShowFill(bool show)
        {
            ShowFill = show;
            OnShowFillChanged?.Invoke();
        }

        public void SetFillAmount(float amount)
        {
            FillAmount = amount;
            OnFillAmountChanged?.Invoke();
        }

        public void SetIcon(Sprite icon)
        {
            Icon = icon;
            OnIconChanged?.Invoke();
        }

        public void SetCount(string countText)
        {
            Count = countText;
            OnCountFieldChanged?.Invoke();
        }

        #if UNITY_EDITOR

        [SerializeField] private float test_fillAmount;
     
        [Button] void Test_Show() => SetShow(true); 
        [Button] void Test_Hide() => SetShow(false); 
        [Button] void Test_ShowFill() => SetShowFill(true);
        [Button] void Test_HideFill() => SetShowFill(false);
        [Button] void Test_SetFillAmount() => SetFillAmount(test_fillAmount);
        #endif
    }
}
