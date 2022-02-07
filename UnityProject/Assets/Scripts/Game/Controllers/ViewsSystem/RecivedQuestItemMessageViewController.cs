using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using UnityEngine;
using Game.Controllers;
using Game.Data;

namespace Game.Data
{
    public class RecivedItemMessageData : IDataViewController
    {
        public Sprite Icon{ get; private set;}
        public string UpperText{ get; private set;}
        public string BottomText{ get; private set;}
        public RecivedItemMessageData(Sprite icon, string upperText, string bottomText)
        {
            Icon = icon;
            UpperText = upperText;
            BottomText = bottomText;
        }
    }
}

namespace Game.Controllers
{

    public class RecivedQuestItemMessageViewController : ViewControllerBase<SpecialMessageView,RecivedItemMessageData>
    {
        [Inject] public QuestsModel QuestsModel { get; private set; }

        protected override void Show() 
        {
            View.SetIcon(Data.Icon);
            View.SetUpperText(Data.UpperText.ToUpper());
            View.SetBottomText(Data.BottomText);
        }

        protected override void Hide() 
        {
        }

    }
}
