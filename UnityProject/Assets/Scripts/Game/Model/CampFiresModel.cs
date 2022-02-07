using System;
using UnityEngine;

namespace Game.Models
{
    public delegate void CookItem(string itemName, int count);

    public class CampFiresModel : MonoBehaviour
    {
        public CampFireModel ActiveCampFire { set; get; }

        public event CookItem OnCookItem;

        public void CookItem(string itemName, int count)
        {
            OnCookItem?.Invoke(itemName, count);
        }
    }
}
