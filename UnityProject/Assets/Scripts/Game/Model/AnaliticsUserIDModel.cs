using System;
using UnityEngine;
using ClipboardExtension;
using Core.Storage;

namespace Game.Models
{
    public class AnaliticsUserIDModel : MonoBehaviour
    {

        [Serializable]
        public class Data : DataBase
        {
            public string ID;

            public void SetID(string id)
            {
                ID = id;
                ChangeData();
            }
        }

        [SerializeField] private Data data = default;

        private const string email = "info@retrostylegames.com";
        
        public Data _Data => data;
        public string ID 
        {
            get{ return data.ID; }
            private set{ data.SetID(value); }
        }

        public event Action OnShowPopup;

        public void SetID(string id) => ID = id;
        
        public void ShowPoupup()
        {
            OnShowPopup?.Invoke();
        }

        public void CopyIDToClipboard() => ID.CopyToClipboard();

        public void SendDataWithEmail()
        {
            string subject = FormatText("User ID");
            string body = FormatText(ID);

            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }
        private string FormatText(string URL)
        {
            return WWW.EscapeURL(URL).Replace("+", "%20");
        }
    }
}