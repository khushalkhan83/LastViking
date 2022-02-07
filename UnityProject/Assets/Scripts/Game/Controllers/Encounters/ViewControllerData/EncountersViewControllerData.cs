using System;
using System.Collections.Generic;
using Game.Controllers;

namespace Game.Encounters.ViewControllerData
{
    public class EncountersViewControllerData: IDataViewController
    {
        public IEnumerable<EncounterViewData> EncountersViewData {get; private set;}
        public IEnumerable<EncounterViewData> TimeEncountersViewData {get; private set;}
        public IEnumerable<EncounterViewData> SpecialEncountersViewData {get; private set;}
        public DiceRollsViewData LocationRollsViewData {get; private set;}
        public DiceRollsViewData TimeRollsViewData {get; private set;}
        public event Action OnViewDataUpdated;
        public event Action<string,bool> OnLocationEncounterCheck;

        public void SetData(IEnumerable<EncounterViewData> encountersViewData,
                            IEnumerable<EncounterViewData> timeEncountersViewData,
                            IEnumerable<EncounterViewData> specialEncountersViewData,
                            DiceRollsViewData locationRollsViewData,
                            DiceRollsViewData timeRollsViewData)
                          
        {
            EncountersViewData = encountersViewData;
            TimeEncountersViewData = timeEncountersViewData;
            SpecialEncountersViewData = specialEncountersViewData;
            LocationRollsViewData = locationRollsViewData;
            TimeRollsViewData = timeRollsViewData;
            OnViewDataUpdated?.Invoke();
        }

        public void PresentEventMessage(string message, bool positive)
        {
            OnLocationEncounterCheck?.Invoke(message,positive);
        }
    }
}
