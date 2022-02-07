using Core.Views;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class EncountersDebugView : ViewBase
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private TextMeshProUGUI status;
        [SerializeField] private TextMeshProUGUI events;
        
        #pragma warning restore 0649
        #endregion

        public string Status {get => status.text; set => status.text = value;}
        public string Events {get => events.text; set => events.text = value;}
    }
}
