using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class VersionInfoView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _versionDescription;

#pragma warning restore 0649
        #endregion

        public void SetDescriptionText(string value) => _versionDescription.text = value;
    }
}
