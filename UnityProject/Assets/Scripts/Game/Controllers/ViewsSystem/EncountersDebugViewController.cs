using Game.Views;
using Core.Controllers;
using Game.Encounters.ViewControllerData;
using System;
using System.Text;
using System.Collections.Generic;

namespace Game.Controllers
{
    public class EncountersDebugViewController : ViewControllerBase<EncountersDebugView,EncountersViewControllerData>
    {
        private List<string> eventMessages = new List<string>();
        private const int k_eventsUpdateCount = 3;
        private int eventsCounter = k_eventsUpdateCount;

        protected override void Show() 
        {
            Data.OnViewDataUpdated += UpdateEvents;
            Data.OnViewDataUpdated += PresentData;
            Data.OnLocationEncounterCheck += AddEvent;

            PresentData();
        }

        protected override void Hide() 
        {
            Data.OnViewDataUpdated -= UpdateEvents;
            Data.OnViewDataUpdated -= PresentData;
            Data.OnLocationEncounterCheck -= AddEvent;
        }


        private void PresentData()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Location encounters");
            SetDiceRollsText(sb,Data.LocationRollsViewData);
            foreach (var viewData in Data.EncountersViewData)
            {
                sb.AppendLine(GetLine(viewData));
            }

            sb.AppendLine();
            sb.AppendLine("Time encounters");
            SetDiceRollsText(sb,Data.TimeRollsViewData);
            foreach (var viewData in Data.TimeEncountersViewData)
            {
                sb.AppendLine(GetLine(viewData));
            }
            
            sb.AppendLine();
            sb.AppendLine("Special encounters");

            foreach (var viewData in Data.SpecialEncountersViewData)
            {
                sb.AppendLine(GetLine(viewData));
            }

            View.Status = sb.ToString();

            PresentEvents();
        }

        private void SetDiceRollsText(StringBuilder stringBuilder, DiceRollsViewData diceRollsViewData)
        {
            stringBuilder.AppendLine(diceRollsViewData.header);
            stringBuilder.AppendLine($"Chance: {diceRollsViewData.chance}");
            stringBuilder.AppendLine($"Last check: {(diceRollsViewData.lastResult.HasValue ? (diceRollsViewData.lastResult.Value ?  "true" : "false") : "None")}");
            stringBuilder.AppendLine($"Dice Rolls Combo: {diceRollsViewData.combo}");
        }

        private void UpdateEvents()
        {
            if(eventMessages.Count == 0) return;

            if(eventsCounter == 0)
            {
                if(eventMessages.Count > 0)
                {
                    eventMessages.RemoveAt(0);
                }

                eventsCounter = k_eventsUpdateCount;
            }
            else
            {
                eventsCounter --;
            }
        }

        private void PresentEvents()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Events:");
            
            foreach (var message in eventMessages)
            {
                sb.AppendLine(message);
            }

            View.Events = sb.ToString();
        }


        private void AddEvent(string message, bool positive)
        {
            eventMessages.Add(GetColorizedMessage());
            PresentEvents();

            string GetColorizedMessage() => $"<color={GetColorCode()}>{message}</color>";
            string GetColorCode() => positive ? "green" : "red";
        }

        private string GetLine(EncounterViewData viewData)
        {
            string answer = $"{viewData.header} {viewData.timeLeft}";
            string colorCode = GetColorCode(viewData.state);

            return $"<color={colorCode}>{answer}</color>";
        }

        private string GetColorCode(ViewState state)
        {
            switch (state)
            {
                case ViewState.active:
                    return "yellow";
                case ViewState.avaliable:
                    return "green";
                case ViewState.notAvaliable:
                    return "red";
                default:
                    return "pink";
            }
        }

    }
}
