using System;
using Game.Controllers;
using UnityEngine;
using Game.Data;

namespace Game.Models
{
    public class SpesialMessagesModel : MonoBehaviour
    {
        public event Action<RecivedItemMessageData> OnRecivedItem;
        
        public void RecivedItem(RecivedItemMessageData data)
        {
            OnRecivedItem?.Invoke(data);
        }
    }
}
