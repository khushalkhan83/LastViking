using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class AutoSaveInfoView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _autoSaveDescription;

        [SerializeField] private Color _successColor;
        [SerializeField] private Color _failColor;

#pragma warning restore 0649
        #endregion

        public Color SuccessColor => _successColor;
        public Color FailColor => _failColor;

        public void SetDescriptionText(string value) => _autoSaveDescription.text = value;
        public void SetColorText(Color color) => _autoSaveDescription.color = color;
    }
}
