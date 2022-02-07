using Extensions;

namespace Game.Views
{
    public class QuestObjectiveView : ObjectiveProcessView
    {
        public void ShowTimeText(bool show) => _timeText.gameObject.CheckNull()?.SetActive(show);
        public void ShowFill(bool show) => _amountImage.gameObject.CheckNull()?.SetActive(show);
        public void ShowSlider(bool show) => _slider.gameObject.CheckNull()?.SetActive(show);
    }
}