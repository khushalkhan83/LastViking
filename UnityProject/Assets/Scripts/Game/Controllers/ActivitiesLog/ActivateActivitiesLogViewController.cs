using System.Collections.Generic;
using ActivitiesLog.ViewControllerData;
using ActivityLog.Data;
using Core;
using Game.Colors;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{

    public class ActivateActivitiesLogViewController : ViewEnableController<ActivitiesLogView>, IActivateActivitiesLogViewController
    {
        [Inject] public ActivitiesLogViewModel ViewModel { get; private set; }
        [Inject] public ActivitiesLogButtonViewModel ButtonViewModel { get; private set; }
        [Inject] public ColorsViewModel ColorsViewModel { get; private set; }
        [Inject] public SideQuestsModel SideQuestsModel { get; private set; }
        [Inject] public EncountersModel EncountersModel { get; private set; }
        [Inject] public DungeonsProgressModel DungeonsProgressModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        public override ViewConfigID ViewConfigID => ViewConfigID.ActivitiesLogConfig;

        // private IActivityLogEnterencesModel SideQuestsActivitiesProducer => SideQuestsModel;
        // private IActivityLogEnterencesModel EncountersModelActivitiesProcuder => EncountersModel;
        // private IActivityLogEnterencesModel DungeonsProgressModelActivitiesProcuder => DungeonsProgressModel;

        private ActivitiesViewData data;

        public override IDataViewController Data { get => data; }

        public override bool IsCanShow => ViewModel.IsShow;

        private const int k_minViewsCount = 4;

        public override void Enable()
        {
            // ViewModel.OnShowChanged += UpdateViewVisible;

            // EncountersModelActivitiesProcuder.OnActivitiesCountChanged += UpdateData;
            // DungeonsProgressModelActivitiesProcuder.OnActivitiesCountChanged += UpdateData;
            // SideQuestsActivitiesProducer.OnActivitiesCountChanged += UpdateData;

            // InitData();
            // data.OnFetchDataRequest += UpdateData;
            // data.OnCloseRequest += CloseViewHandler;
            // UpdateData();

            // UpdateViewVisible();
        }
        public override void Disable()
        {
            // ViewModel.OnShowChanged -= UpdateViewVisible;

            // EncountersModelActivitiesProcuder.OnActivitiesCountChanged -= UpdateData;
            // DungeonsProgressModelActivitiesProcuder.OnActivitiesCountChanged -= UpdateData;
            // SideQuestsActivitiesProducer.OnActivitiesCountChanged -= UpdateData;

            // data.OnCloseRequest -= CloseViewHandler;
            // data.OnFetchDataRequest -= UpdateData;

            // Hide();
        }

        public override void Start() { }

        private void InitData()
        {
            data = new ActivitiesViewData();
        }

        private void UpdateData()
        {
            var viewDatas = GetViewDatas();
            var realViewsCount = viewDatas.Count;
            
            if(realViewsCount < k_minViewsCount)
            {
                var dif = k_minViewsCount - realViewsCount;
                viewDatas.AddRange(GetPlaceholderViewDatas(dif,ColorPreset.darkGray,ColorPreset.gray));
            }
            data.SetViewDatas(viewDatas);

            ButtonViewModel.SetCounter(realViewsCount);
        }

        private void CloseViewHandler() => ViewModel.SetShow(false);

        private List<ViewData> GetViewDatas()
        {
            var answer = new List<ViewData>();

            // answer.AddRange(GetViewDatas(SideQuestsActivitiesProducer.GetActivitiesEnterences(), ColorPreset.green, ColorPreset.spectra));
            // answer.AddRange(GetViewDatas(DungeonsProgressModelActivitiesProcuder.GetActivitiesEnterences(), ColorPreset.blue, ColorPreset.spectra));
            // answer.AddRange(GetViewDatas(EncountersModelActivitiesProcuder.GetActivitiesEnterences(), ColorPreset.blue, ColorPreset.spectra));

            return answer;
        }

        private List<ViewData> GetViewDatas(List<ActivityLogEnterenceData> activities, Colors.ColorPreset textBackground, ColorPreset iconBacground)
        {
            var answer = new List<ViewData>();
            foreach (var activity in activities)
            {
                var textBacgroundColor = ColorsViewModel.GetColor(textBackground);
                var iconBackgroundColor = ColorsViewModel.GetColor(iconBacground);
                answer.Add(new ViewData(activity.Icon, activity.Message.Invoke(), textBacgroundColor,iconBackgroundColor));
            }

            return answer;
        }

        private List<ViewData> GetPlaceholderViewDatas(int count, ColorPreset textBackground, ColorPreset iconBackground)
        {
            var answer = new List<ViewData>();

            for (int i = 0; i < count; i++)
            {
                var textBacgroundColor = ColorsViewModel.GetColor(textBackground);
                var iconBackgroundColor = ColorsViewModel.GetColor(iconBackground);
                answer.Add(new ViewData(ViewModel.NextUpdateIcon, PlaceholderText(), textBacgroundColor, iconBackgroundColor, 0.75f));
            }

            return answer;
        }

        private string PlaceholderText() => LocalizationModel.GetString(LocalizationKeyID.Activity_waitNextUpdate);
    }
}