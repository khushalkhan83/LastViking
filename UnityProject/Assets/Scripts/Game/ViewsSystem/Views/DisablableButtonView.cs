using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DisablableButtonView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _ActiveObject;

        [VisibleObject]
        [SerializeField] private GameObject _passiveObject;

        [SerializeField] private Text _activeButtonText;
        [SerializeField] private Text _passiveButtonText;

#pragma warning restore 0649
        #endregion

        public void SetIsVisibleActiveObject(bool value) => _ActiveObject.SetActive(value);
        public void SetIsVisiblePassiveObject(bool value) => _passiveObject.SetActive(value);

        public void SetIsVisible(bool value)
        {
            SetIsVisibleActiveObject(value);
            SetIsVisiblePassiveObject(!value);
        }

        public void SetText(string text)
        {
            _activeButtonText.text = text;
            _passiveButtonText.text = text;
        }
    }
}
