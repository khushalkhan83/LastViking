using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class HouseAttackedInfoView : ViewAnimateBase
    {
        #region Data
#pragma warning disable 0649

        //Localization text targets
        [SerializeField] private Text _message;

#pragma warning restore 0649
        #endregion

        public void SetTextMessage(string text)
        {
            _message.text = text;
        }
    }
}
