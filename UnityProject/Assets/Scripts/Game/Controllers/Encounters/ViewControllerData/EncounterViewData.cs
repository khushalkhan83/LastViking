using System;
using Game.Controllers;
using UnityEngine;

namespace Game.Encounters.ViewControllerData
{
    public enum ViewState
    {
        avaliable,
        active,
        notAvaliable
    }
    public class EncounterViewData
    {
        public readonly string header;
        public readonly string timeLeft;
        public readonly ViewState state;

        public EncounterViewData(string header, string timeLeft, ViewState state)
        {
            this.header = header;
            this.timeLeft = timeLeft;
            this.state = state;
        }
    }
}
