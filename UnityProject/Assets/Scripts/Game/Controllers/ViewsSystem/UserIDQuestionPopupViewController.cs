using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using System.Text;
using UnityEngine;

namespace Game.Controllers
{
    public class UserIDQuestionPopupViewController : ViewControllerBase<QuestionPopupView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AnaliticsUserIDModel AnaliticsUserIDModel { get; private set; }

        protected override void Show() 
        {
            SetText();
            View.OnApply += SendEmailButtonHandler;
            View.OnClose += CopyButtonHandler;
        }

        protected override void Hide() 
        {
            View.OnApply -= SendEmailButtonHandler;
            View.OnClose -= CopyButtonHandler;
        }

        private void SendEmailButtonHandler()
        {
            AnaliticsUserIDModel.CopyIDToClipboard();
            try
            {
                AnaliticsUserIDModel.SendDataWithEmail();
            }
            catch (System.Exception)
            {
                Debug.LogError("Can`t send code");    
            }

            CloseView();
        }

        private void CopyButtonHandler()
        {
            AnaliticsUserIDModel.CopyIDToClipboard();
            CloseView();
        }


        private void SetText()
        {
            View.SetTextTitle("Notification");
            View.SetTextDescription(GetNoficicationMessage());
            View.SetTextOkButton("Send Email");
            View.SetTextBackButton("Copy");
        }

        private string GetNoficicationMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your id is :");
            sb.AppendLine(AnaliticsUserIDModel.ID);
            return sb.ToString();
        }

        private void CloseView() => ViewsSystem.Hide(View);
    }
}
