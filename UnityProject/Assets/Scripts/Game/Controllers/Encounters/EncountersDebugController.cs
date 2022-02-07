using System;
using System.Collections.Generic;
using Chances;
using Core;
using Encounters;
using Extensions;
using Game.Encounters.ViewControllerData;
using Game.Models;
using Game.Views;
using Updaters;

namespace Game.Controllers
{
    public class EncountersDebugController : ViewEnableController<EncountersDebugView>, IEncountersDebugController
    {
        [Inject] public EncountersViewModel ViewModel { get; private set; }
        [Inject] public EncountersModel EncountersModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        
        public override ViewConfigID ViewConfigID => ViewConfigID.EncountersDebugConfig;
        public override bool IsCanShow => ViewModel.IsShow;
        private EncountersViewControllerData data { get; set; }
        public override IDataViewController Data { get => data; }

        private IUpdater debugInfoUpdater;
        private const float updateDebugDataTime = 0.5f;

        private List<IEncounter> LocationEncounters {get => EncountersModel.LocationEncounters;}
        private List<IEncounter> TimeEncounters {get => EncountersModel.TimeEncounters;}
        private List<ISpecialEncounter> SpecialEncounters {get => EncountersModel.SpecialEncounters;}

        private IProgressiveSmartDice locationEncounterDice {get => EncountersModel.locationEncounterDice;}
        private IProgressiveSmartDice timeEncounterDice {get => EncountersModel.timeEncounterDice;}

        
        public override void Enable()
        {
            debugInfoUpdater = new DelayedUpdater(updateDebugDataTime,UpdateViewData);
            
            ViewModel.OnShowChanged += UpdateViewVisible;
            ViewModel.OnPresentEventMessage += OnPresentEventMessage;
            ViewModel.OnRefresh += UpdateViewData;
            GameUpdateModel.OnUpdate += debugInfoUpdater.Tick;

            InitViewControllerData();

            UpdateViewVisible();
        }

        public override void Start() { }
        public override void Disable()
        {
            ViewModel.OnShowChanged -= UpdateViewVisible;
            ViewModel.OnPresentEventMessage -= OnPresentEventMessage;
            ViewModel.OnRefresh -= UpdateViewData;
            GameUpdateModel.OnUpdate -= debugInfoUpdater.Tick;
            Hide();
        }

        private void InitViewControllerData()
        {
            data = CreateViewControllerData();
            UpdateViewData();
        }

        private EncountersViewControllerData CreateViewControllerData()
        {
            var answer = new EncountersViewControllerData();
            return answer;
        }

        private void UpdateViewData() => data.SetData(GetRandomEncountersViewData(),
                                                      GetTimeEncountersViewData(),
                                                      GetSpecialEncountersViewData(),
                                                      GetRollsViewData("location dice rolls",locationEncounterDice),
                                                      GetRollsViewData("time dice rolls",timeEncounterDice)
                                                      );


        private void OnPresentEventMessage(string message, bool isPositive) => data.PresentEventMessage(message,isPositive);


        private IEnumerable<EncounterViewData> GetRandomEncountersViewData() => GetEncounterViewData("location",LocationEncounters);
        private IEnumerable<EncounterViewData> GetTimeEncountersViewData() => GetEncounterViewData("time",TimeEncounters);
        private IEnumerable<EncounterViewData> GetSpecialEncountersViewData() => GetEncounterViewData("special",SpecialEncounters);

        private IEnumerable<EncounterViewData> GetEncounterViewData<T>(string header, List<T> encounters) where T: IEncounter
        {
            var answer = new List<EncounterViewData>();
            int index = 0;
            foreach (var encounter in encounters)
            {
                answer.Add(new EncounterViewData($"{header} {index} {encounter.GetType().Name}: chance {encounter.chanceWeight}",
                                        $"cooldown: {encounter.cooldown.GetFormatedSeconds()}",
                                        encounter.IsActive ? ViewState.active : encounter.CanOccure ? ViewState.avaliable : ViewState.notAvaliable));

                index++;
            }

            return answer;
        }

        private DiceRollsViewData GetRollsViewData(string message, IProgressiveSmartDice dice)
        {
            return new DiceRollsViewData(message,dice.Chance, dice.LastResult, dice.ResultCombo);
        }
    }
}